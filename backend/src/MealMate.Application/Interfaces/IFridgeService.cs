using MealMate.Application.Common;
using MealMate.Application.DTOs.Fridge;

namespace MealMate.Application.Interfaces;

public interface IFridgeService
{
    Task<ServiceResult<IEnumerable<FridgeItemDto>>> GetByHouseholdAsync(Guid householdId, Guid userId);
    Task<ServiceResult<FridgeItemDto>> AddItemAsync(Guid householdId, CreateFridgeItemDto dto, Guid userId);
    Task<ServiceResult<FridgeItemDto>> UpdateItemAsync(Guid itemId, UpdateFridgeItemDto dto, Guid userId);
    Task<ServiceResult<bool>> DeleteItemAsync(Guid itemId, Guid userId);
}
