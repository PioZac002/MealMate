using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MealMate.Application.Common;
using MealMate.Application.DTOs.Ingredient;
using MealMate.Application.Interfaces;
using MealMate.Domain.Entities;

namespace MealMate.Application.Services;

public interface IIngredientDbContext
{
    Microsoft.EntityFrameworkCore.DbSet<Ingredient> Ingredients { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public class IngredientService : IIngredientService
{
    private readonly IIngredientDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<IngredientService> _logger;

    public IngredientService(IIngredientDbContext context, IMapper mapper, ILogger<IngredientService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ServiceResult<PagedResult<IngredientDto>>> GetAllAsync(IngredientFilterDto filter)
    {
        var query = _context.Ingredients.AsQueryable();

        if (!string.IsNullOrWhiteSpace(filter.Search))
            query = query.Where(i => i.Name.ToLower().Contains(filter.Search.ToLower()));

        if (filter.Category.HasValue)
            query = query.Where(i => i.Category == filter.Category.Value);

        var total = await query.CountAsync();
        var items = await query
            .OrderBy(i => i.Name)
            .Skip((filter.Page - 1) * filter.PageSize)
            .Take(filter.PageSize)
            .ToListAsync();

        return ServiceResult<PagedResult<IngredientDto>>.Ok(new PagedResult<IngredientDto>
        {
            Items = _mapper.Map<IEnumerable<IngredientDto>>(items),
            TotalCount = total,
            Page = filter.Page,
            PageSize = filter.PageSize
        });
    }

    public async Task<ServiceResult<IngredientDto>> GetByIdAsync(Guid id)
    {
        var ingredient = await _context.Ingredients.FindAsync(id);
        if (ingredient == null)
            return ServiceResult<IngredientDto>.NotFound("Ingredient not found.");

        return ServiceResult<IngredientDto>.Ok(_mapper.Map<IngredientDto>(ingredient));
    }

    public async Task<ServiceResult<IngredientDto>> CreateAsync(CreateIngredientDto dto)
    {
        var exists = await _context.Ingredients.AnyAsync(i => i.Name.ToLower() == dto.Name.ToLower());
        if (exists)
            return ServiceResult<IngredientDto>.Fail("An ingredient with this name already exists.");

        var ingredient = _mapper.Map<Ingredient>(dto);
        ingredient.Id = Guid.NewGuid();
        _context.Ingredients.Add(ingredient);
        await _context.SaveChangesAsync();
        return ServiceResult<IngredientDto>.Created(_mapper.Map<IngredientDto>(ingredient));
    }

    public async Task<ServiceResult<IngredientDto>> UpdateAsync(Guid id, UpdateIngredientDto dto)
    {
        var ingredient = await _context.Ingredients.FindAsync(id);
        if (ingredient == null)
            return ServiceResult<IngredientDto>.NotFound("Ingredient not found.");

        _mapper.Map(dto, ingredient);
        await _context.SaveChangesAsync();
        return ServiceResult<IngredientDto>.Ok(_mapper.Map<IngredientDto>(ingredient));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Guid id)
    {
        var ingredient = await _context.Ingredients.FindAsync(id);
        if (ingredient == null)
            return ServiceResult<bool>.NotFound("Ingredient not found.");

        _context.Ingredients.Remove(ingredient);
        await _context.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }
}
