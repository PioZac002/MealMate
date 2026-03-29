using MealMate.Application.Common;
using MealMate.Application.DTOs.Ingredient;

namespace MealMate.Application.Interfaces;

public interface IIngredientService
{
    Task<ServiceResult<PagedResult<IngredientDto>>> GetAllAsync(IngredientFilterDto filter);
    Task<ServiceResult<IngredientDto>> GetByIdAsync(Guid id);
    Task<ServiceResult<IngredientDto>> CreateAsync(CreateIngredientDto dto);
    Task<ServiceResult<IngredientDto>> UpdateAsync(Guid id, UpdateIngredientDto dto);
    Task<ServiceResult<bool>> DeleteAsync(Guid id);
}
