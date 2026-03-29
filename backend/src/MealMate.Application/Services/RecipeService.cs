using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MealMate.Application.Common;
using MealMate.Application.DTOs.Recipe;
using MealMate.Application.Interfaces;
using MealMate.Domain.Entities;
using MealMate.Domain.Enums;

namespace MealMate.Application.Services;

public interface IRecipeDbContext
{
    Microsoft.EntityFrameworkCore.DbSet<Recipe> Recipes { get; }
    Microsoft.EntityFrameworkCore.DbSet<RecipeIngredient> RecipeIngredients { get; }
    Microsoft.EntityFrameworkCore.DbSet<RecipeStep> RecipeSteps { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public class RecipeService : IRecipeService
{
    private readonly IRecipeDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<RecipeService> _logger;

    public RecipeService(IRecipeDbContext context, IMapper mapper, ILogger<RecipeService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ServiceResult<PagedResult<RecipeDto>>> GetAllAsync(RecipeFilterDto filter, Guid userId)
    {
        var query = _context.Recipes
            .Include(r => r.CreatedByUser)
            .Where(r => r.IsPublic || r.CreatedByUserId == userId);

        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(r => r.Title.ToLower().Contains(filter.Search.ToLower()));

        if (filter.DietType.HasValue)
            query = query.Where(r => r.DietType == filter.DietType.Value);

        if (filter.MaxPrepTime.HasValue)
            query = query.Where(r => r.PrepTimeMinutes <= filter.MaxPrepTime.Value);

        if (filter.MaxCookTime.HasValue)
            query = query.Where(r => r.CookTimeMinutes <= filter.MaxCookTime.Value);

        var total = await query.CountAsync();
        var items = await query
            .OrderByDescending(r => r.CreatedAt)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return ServiceResult<PagedResult<RecipeDto>>.Ok(new PagedResult<RecipeDto>
        {
            Items = _mapper.Map<IEnumerable<RecipeDto>>(items),
            TotalCount = total,
            Page = filter.Page,
            PageSize = filter.PageSize
        });
    }

    public async Task<ServiceResult<RecipeDetailDto>> GetByIdAsync(Guid id, Guid userId)
    {
        var recipe = await _context.Recipes
            .Include(r => r.CreatedByUser)
            .Include(r => r.Ingredients).ThenInclude(i => i.Ingredient)
            .Include(r => r.Steps)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (recipe == null)
            return ServiceResult<RecipeDetailDto>.NotFound("Recipe not found.");

        if (!recipe.IsPublic && recipe.CreatedByUserId != userId)
            return ServiceResult<RecipeDetailDto>.Forbidden("This recipe is private.");

        var dto = _mapper.Map<RecipeDetailDto>(recipe);
        CalculateNutrition(dto, recipe);
        return ServiceResult<RecipeDetailDto>.Ok(dto);
    }

    public async Task<ServiceResult<RecipeDetailDto>> CreateAsync(CreateRecipeDto dto, Guid userId)
    {
        var recipe = new Recipe
        {
            Id = Guid.NewGuid(),
            Title = dto.Title,
            Description = dto.Description,
            PrepTimeMinutes = dto.PrepTimeMinutes,
            CookTimeMinutes = dto.CookTimeMinutes,
            Servings = dto.Servings,
            DietType = dto.DietType,
            ImageUrl = dto.ImageUrl,
            IsPublic = dto.IsPublic,
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow,
            Ingredients = dto.Ingredients.Select(i => new RecipeIngredient
            {
                Id = Guid.NewGuid(),
                IngredientId = i.IngredientId,
                Quantity = i.Quantity,
                Unit = i.Unit
            }).ToList(),
            Steps = dto.Steps.Select(s => new RecipeStep
            {
                Id = Guid.NewGuid(),
                StepNumber = s.StepNumber,
                Description = s.Description,
                ImageUrl = s.ImageUrl
            }).ToList()
        };

        _context.Recipes.Add(recipe);
        await _context.SaveChangesAsync();

        return await GetByIdAsync(recipe.Id, userId);
    }

    public async Task<ServiceResult<RecipeDetailDto>> UpdateAsync(Guid id, UpdateRecipeDto dto, Guid userId)
    {
        var recipe = await _context.Recipes
            .Include(r => r.Ingredients)
            .Include(r => r.Steps)
            .FirstOrDefaultAsync(r => r.Id == id);

        if (recipe == null)
            return ServiceResult<RecipeDetailDto>.NotFound("Recipe not found.");

        if (recipe.CreatedByUserId != userId)
            return ServiceResult<RecipeDetailDto>.Forbidden("You can only update your own recipes.");

        recipe.Title = dto.Title;
        recipe.Description = dto.Description;
        recipe.PrepTimeMinutes = dto.PrepTimeMinutes;
        recipe.CookTimeMinutes = dto.CookTimeMinutes;
        recipe.Servings = dto.Servings;
        recipe.DietType = dto.DietType;
        recipe.ImageUrl = dto.ImageUrl;
        recipe.IsPublic = dto.IsPublic;

        _context.RecipeIngredients.RemoveRange(recipe.Ingredients);
        _context.RecipeSteps.RemoveRange(recipe.Steps);

        recipe.Ingredients = dto.Ingredients.Select(i => new RecipeIngredient
        {
            Id = Guid.NewGuid(),
            RecipeId = recipe.Id,
            IngredientId = i.IngredientId,
            Quantity = i.Quantity,
            Unit = i.Unit
        }).ToList();

        recipe.Steps = dto.Steps.Select(s => new RecipeStep
        {
            Id = Guid.NewGuid(),
            RecipeId = recipe.Id,
            StepNumber = s.StepNumber,
            Description = s.Description,
            ImageUrl = s.ImageUrl
        }).ToList();

        await _context.SaveChangesAsync();
        return await GetByIdAsync(recipe.Id, userId);
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Guid id, Guid userId)
    {
        var recipe = await _context.Recipes.FindAsync(id);
        if (recipe == null)
            return ServiceResult<bool>.NotFound("Recipe not found.");

        if (recipe.CreatedByUserId != userId)
            return ServiceResult<bool>.Forbidden("You can only delete your own recipes.");

        _context.Recipes.Remove(recipe);
        await _context.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }

    private static void CalculateNutrition(RecipeDetailDto dto, Recipe recipe)
    {
        decimal totalCalories = 0, totalProtein = 0, totalCarbs = 0, totalFat = 0;
        foreach (var ri in recipe.Ingredients)
        {
            if (ri.Ingredient == null) continue;
            var factor = ri.Quantity / 100m;
            totalCalories += ri.Ingredient.CaloriesPer100g * factor;
            totalProtein += ri.Ingredient.ProteinPer100g * factor;
            totalCarbs += ri.Ingredient.CarbsPer100g * factor;
            totalFat += ri.Ingredient.FatPer100g * factor;
        }
        dto.TotalCalories = Math.Round(totalCalories, 1);
        dto.TotalProtein = Math.Round(totalProtein, 1);
        dto.TotalCarbs = Math.Round(totalCarbs, 1);
        dto.TotalFat = Math.Round(totalFat, 1);
    }
}
