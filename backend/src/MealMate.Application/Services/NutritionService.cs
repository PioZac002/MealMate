using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MealMate.Application.Common;
using MealMate.Application.DTOs.Nutrition;
using MealMate.Application.Interfaces;
using MealMate.Domain.Entities;
using MealMate.Domain.Enums;

namespace MealMate.Application.Services;

public interface INutritionDbContext
{
    DbSet<DailyNutritionLog> DailyNutritionLogs { get; }
    DbSet<MealLog> MealLogs { get; }
    DbSet<Recipe> Recipes { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public class NutritionService : INutritionService
{
    private readonly INutritionDbContext _db;
    private readonly IMapper _mapper;

    public NutritionService(INutritionDbContext db, IMapper mapper)
    {
        _db = db;
        _mapper = mapper;
    }

    public async Task<ServiceResult<DailyNutritionLogDto>> GetOrCreateLogAsync(DateOnly date, Guid userId)
    {
        var log = await _db.DailyNutritionLogs
            .Include(l => l.MealLogs)
                .ThenInclude(m => m.Recipe)
            .FirstOrDefaultAsync(l => l.UserId == userId && l.Date == date);

        if (log == null)
        {
            log = new DailyNutritionLog
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Date = date,
                CalorieGoal = 2000,
                ProteinGoal = 150,
                CarbsGoal = 250,
                FatGoal = 65
            };
            _db.DailyNutritionLogs.Add(log);
            await _db.SaveChangesAsync();
        }

        return ServiceResult<DailyNutritionLogDto>.Ok(MapLog(log));
    }

    public async Task<ServiceResult<DailyNutritionLogDto>> SetGoalsAsync(DateOnly date, SetGoalsDto dto, Guid userId)
    {
        var log = await _db.DailyNutritionLogs
            .Include(l => l.MealLogs)
                .ThenInclude(m => m.Recipe)
            .FirstOrDefaultAsync(l => l.UserId == userId && l.Date == date);

        if (log == null)
        {
            log = new DailyNutritionLog
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Date = date,
                CalorieGoal = dto.CalorieGoal,
                ProteinGoal = dto.ProteinGoal,
                CarbsGoal = dto.CarbsGoal,
                FatGoal = dto.FatGoal,
                Notes = dto.Notes
            };
            _db.DailyNutritionLogs.Add(log);
        }
        else
        {
            log.CalorieGoal = dto.CalorieGoal;
            log.ProteinGoal = dto.ProteinGoal;
            log.CarbsGoal = dto.CarbsGoal;
            log.FatGoal = dto.FatGoal;
            log.Notes = dto.Notes;
        }

        await _db.SaveChangesAsync();
        return ServiceResult<DailyNutritionLogDto>.Ok(MapLog(log));
    }

    public async Task<ServiceResult<MealLogDto>> AddMealLogAsync(DateOnly date, AddMealLogDto dto, Guid userId)
    {
        if (!Enum.TryParse<MealType>(dto.MealType, ignoreCase: true, out var mealType))
            return ServiceResult<MealLogDto>.Fail("Invalid meal type");

        var log = await _db.DailyNutritionLogs
            .FirstOrDefaultAsync(l => l.UserId == userId && l.Date == date);

        if (log == null)
        {
            log = new DailyNutritionLog
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Date = date,
                CalorieGoal = 2000,
                ProteinGoal = 150,
                CarbsGoal = 250,
                FatGoal = 65
            };
            _db.DailyNutritionLogs.Add(log);
            await _db.SaveChangesAsync();
        }

        string? recipeName = null;
        if (dto.RecipeId.HasValue)
        {
            var recipe = await _db.Recipes.FindAsync(dto.RecipeId.Value);
            recipeName = recipe?.Title;
        }

        var mealLog = new MealLog
        {
            Id = Guid.NewGuid(),
            DailyNutritionLogId = log.Id,
            MealType = mealType,
            RecipeId = dto.RecipeId,
            CustomFoodName = dto.CustomFoodName,
            Calories = dto.Calories,
            Protein = dto.Protein,
            Carbs = dto.Carbs,
            Fat = dto.Fat,
            Servings = dto.Servings,
            LoggedAt = DateTime.UtcNow
        };

        _db.MealLogs.Add(mealLog);
        await _db.SaveChangesAsync();

        var result = _mapper.Map<MealLogDto>(mealLog);
        result.RecipeName = recipeName;
        return ServiceResult<MealLogDto>.Ok(result);
    }

    public async Task<ServiceResult<bool>> RemoveMealLogAsync(Guid mealLogId, Guid userId)
    {
        var mealLog = await _db.MealLogs
            .Include(m => m.DailyNutritionLog)
            .FirstOrDefaultAsync(m => m.Id == mealLogId);

        if (mealLog == null)
            return ServiceResult<bool>.NotFound("Meal log not found");

        if (mealLog.DailyNutritionLog.UserId != userId)
            return ServiceResult<bool>.Forbidden();

        _db.MealLogs.Remove(mealLog);
        await _db.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }

    public async Task<ServiceResult<IEnumerable<DailyNutritionLogDto>>> GetHistoryAsync(Guid userId, int days)
    {
        var from = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-days));
        var logs = await _db.DailyNutritionLogs
            .Include(l => l.MealLogs)
                .ThenInclude(m => m.Recipe)
            .Where(l => l.UserId == userId && l.Date >= from)
            .OrderByDescending(l => l.Date)
            .ToListAsync();

        return ServiceResult<IEnumerable<DailyNutritionLogDto>>.Ok(logs.Select(MapLog));
    }

    private DailyNutritionLogDto MapLog(DailyNutritionLog log)
    {
        return _mapper.Map<DailyNutritionLogDto>(log);
    }
}
