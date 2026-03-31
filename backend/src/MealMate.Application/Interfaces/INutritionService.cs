using MealMate.Application.Common;
using MealMate.Application.DTOs.Nutrition;

namespace MealMate.Application.Interfaces;

public interface INutritionService
{
    Task<ServiceResult<DailyNutritionLogDto>> GetOrCreateLogAsync(DateOnly date, Guid userId);
    Task<ServiceResult<DailyNutritionLogDto>> SetGoalsAsync(DateOnly date, SetGoalsDto dto, Guid userId);
    Task<ServiceResult<MealLogDto>> AddMealLogAsync(DateOnly date, AddMealLogDto dto, Guid userId);
    Task<ServiceResult<bool>> RemoveMealLogAsync(Guid mealLogId, Guid userId);
    Task<ServiceResult<IEnumerable<DailyNutritionLogDto>>> GetHistoryAsync(Guid userId, int days);
}
