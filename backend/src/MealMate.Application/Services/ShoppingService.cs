using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MealMate.Application.Common;
using MealMate.Application.DTOs.Shopping;
using MealMate.Application.Interfaces;
using MealMate.Domain.Entities;
using MealMate.Domain.Enums;

namespace MealMate.Application.Services;

public interface IShoppingDbContext
{
    DbSet<ShoppingList> ShoppingLists { get; }
    DbSet<ShoppingListItem> ShoppingListItems { get; }
    DbSet<HouseholdMember> HouseholdMembers { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public class ShoppingService : IShoppingService
{
    private readonly IShoppingDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<ShoppingService> _logger;

    public ShoppingService(IShoppingDbContext context, IMapper mapper, ILogger<ShoppingService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ServiceResult<IEnumerable<ShoppingListDto>>> GetByHouseholdAsync(Guid householdId, Guid userId)
    {
        if (!await IsMemberAsync(householdId, userId))
            return ServiceResult<IEnumerable<ShoppingListDto>>.Forbidden("You are not a member of this household.");

        var lists = await _context.ShoppingLists
            .Include(s => s.Items)
            .Where(s => s.HouseholdId == householdId)
            .OrderByDescending(s => s.CreatedAt)
            .ToListAsync();

        return ServiceResult<IEnumerable<ShoppingListDto>>.Ok(_mapper.Map<IEnumerable<ShoppingListDto>>(lists));
    }

    public async Task<ServiceResult<ShoppingListDetailDto>> GetByIdAsync(Guid listId, Guid userId)
    {
        var list = await _context.ShoppingLists
            .Include(s => s.Items).ThenInclude(i => i.Ingredient)
            .FirstOrDefaultAsync(s => s.Id == listId);

        if (list == null)
            return ServiceResult<ShoppingListDetailDto>.NotFound("Shopping list not found.");

        if (!await IsMemberAsync(list.HouseholdId, userId))
            return ServiceResult<ShoppingListDetailDto>.Forbidden("You are not a member of this household.");

        return ServiceResult<ShoppingListDetailDto>.Ok(_mapper.Map<ShoppingListDetailDto>(list));
    }

    public async Task<ServiceResult<ShoppingListDetailDto>> CreateAsync(Guid householdId, CreateShoppingListDto dto, Guid userId)
    {
        if (!await IsMemberAsync(householdId, userId))
            return ServiceResult<ShoppingListDetailDto>.Forbidden("You are not a member of this household.");

        var list = new ShoppingList
        {
            Id = Guid.NewGuid(),
            HouseholdId = householdId,
            Name = dto.Name,
            CreatedAt = DateTime.UtcNow
        };

        _context.ShoppingLists.Add(list);
        await _context.SaveChangesAsync();

        return ServiceResult<ShoppingListDetailDto>.Ok(_mapper.Map<ShoppingListDetailDto>(list));
    }

    public async Task<ServiceResult<ShoppingListItemDto>> AddItemAsync(Guid listId, AddShoppingListItemDto dto, Guid userId)
    {
        var list = await _context.ShoppingLists.FindAsync(listId);
        if (list == null)
            return ServiceResult<ShoppingListItemDto>.NotFound("Shopping list not found.");

        if (!await IsMemberAsync(list.HouseholdId, userId))
            return ServiceResult<ShoppingListItemDto>.Forbidden("You are not a member of this household.");

        var item = new ShoppingListItem
        {
            Id = Guid.NewGuid(),
            ShoppingListId = listId,
            IngredientId = dto.IngredientId,
            Quantity = dto.Quantity,
            Unit = dto.Unit,
            Source = ShoppingItemSource.Manual
        };

        _context.ShoppingListItems.Add(item);
        await _context.SaveChangesAsync();

        var created = await _context.ShoppingListItems
            .Include(i => i.Ingredient)
            .FirstAsync(i => i.Id == item.Id);

        return ServiceResult<ShoppingListItemDto>.Ok(_mapper.Map<ShoppingListItemDto>(created));
    }

    public async Task<ServiceResult<ShoppingListItemDto>> ToggleItemAsync(Guid listId, Guid itemId, Guid userId)
    {
        var item = await _context.ShoppingListItems
            .Include(i => i.Ingredient)
            .FirstOrDefaultAsync(i => i.Id == itemId && i.ShoppingListId == listId);

        if (item == null)
            return ServiceResult<ShoppingListItemDto>.NotFound("Shopping list item not found.");

        var list = await _context.ShoppingLists.FindAsync(listId);
        if (!await IsMemberAsync(list!.HouseholdId, userId))
            return ServiceResult<ShoppingListItemDto>.Forbidden("You are not a member of this household.");

        item.IsBought = !item.IsBought;
        item.BoughtByUserId = item.IsBought ? userId : null;
        item.BoughtAt = item.IsBought ? DateTime.UtcNow : null;

        await _context.SaveChangesAsync();

        return ServiceResult<ShoppingListItemDto>.Ok(_mapper.Map<ShoppingListItemDto>(item));
    }

    public async Task<ServiceResult<bool>> RemoveItemAsync(Guid listId, Guid itemId, Guid userId)
    {
        var item = await _context.ShoppingListItems
            .FirstOrDefaultAsync(i => i.Id == itemId && i.ShoppingListId == listId);

        if (item == null)
            return ServiceResult<bool>.NotFound("Shopping list item not found.");

        var list = await _context.ShoppingLists.FindAsync(listId);
        if (!await IsMemberAsync(list!.HouseholdId, userId))
            return ServiceResult<bool>.Forbidden("You are not a member of this household.");

        _context.ShoppingListItems.Remove(item);
        await _context.SaveChangesAsync();

        return ServiceResult<bool>.Ok(true);
    }

    public async Task<ServiceResult<ShoppingListDetailDto>> CompleteListAsync(Guid listId, Guid userId)
    {
        var list = await _context.ShoppingLists
            .Include(s => s.Items).ThenInclude(i => i.Ingredient)
            .FirstOrDefaultAsync(s => s.Id == listId);

        if (list == null)
            return ServiceResult<ShoppingListDetailDto>.NotFound("Shopping list not found.");

        if (!await IsMemberAsync(list.HouseholdId, userId))
            return ServiceResult<ShoppingListDetailDto>.Forbidden("You are not a member of this household.");

        list.IsCompleted = true;
        list.CompletedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return ServiceResult<ShoppingListDetailDto>.Ok(_mapper.Map<ShoppingListDetailDto>(list));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Guid listId, Guid userId)
    {
        var list = await _context.ShoppingLists.FindAsync(listId);
        if (list == null)
            return ServiceResult<bool>.NotFound("Shopping list not found.");

        if (!await IsMemberAsync(list.HouseholdId, userId))
            return ServiceResult<bool>.Forbidden("You are not a member of this household.");

        _context.ShoppingLists.Remove(list);
        await _context.SaveChangesAsync();

        return ServiceResult<bool>.Ok(true);
    }

    private async Task<bool> IsMemberAsync(Guid householdId, Guid userId)
    {
        return await _context.HouseholdMembers
            .AnyAsync(m => m.HouseholdId == householdId && m.UserId == userId);
    }
}
