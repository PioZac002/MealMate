using MealMate.Application.Common;
using MealMate.Application.DTOs.Recipe;

namespace MealMate.Application.Interfaces;

public interface IRecipeService
{
    Task<ServiceResult<PagedResult<RecipeDto>>> GetAllAsync(RecipeFilterDto filter, Guid userId);
    Task<ServiceResult<RecipeDetailDto>> GetByIdAsync(Guid id, Guid userId);
    Task<ServiceResult<RecipeDetailDto>> CreateAsync(CreateRecipeDto dto, Guid userId);
    Task<ServiceResult<RecipeDetailDto>> UpdateAsync(Guid id, UpdateRecipeDto dto, Guid userId);
    Task<ServiceResult<bool>> DeleteAsync(Guid id, Guid userId);
}
