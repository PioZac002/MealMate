using MealMate.Application.Common;
using MealMate.Application.DTOs.Shopping;

namespace MealMate.Application.Interfaces;

public interface IShoppingService
{
    Task<ServiceResult<IEnumerable<ShoppingListDto>>> GetByHouseholdAsync(Guid householdId, Guid userId);
    Task<ServiceResult<ShoppingListDetailDto>> GetByIdAsync(Guid listId, Guid userId);
    Task<ServiceResult<ShoppingListDetailDto>> CreateAsync(Guid householdId, CreateShoppingListDto dto, Guid userId);
    Task<ServiceResult<ShoppingListItemDto>> AddItemAsync(Guid listId, AddShoppingListItemDto dto, Guid userId);
    Task<ServiceResult<ShoppingListItemDto>> ToggleItemAsync(Guid listId, Guid itemId, Guid userId);
    Task<ServiceResult<bool>> RemoveItemAsync(Guid listId, Guid itemId, Guid userId);
    Task<ServiceResult<ShoppingListDetailDto>> CompleteListAsync(Guid listId, Guid userId);
    Task<ServiceResult<bool>> DeleteAsync(Guid listId, Guid userId);
}
