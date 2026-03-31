using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MealMate.Application.Common;
using MealMate.Application.DTOs.Fridge;
using MealMate.Application.Interfaces;
using MealMate.Domain.Entities;
using MealMate.Domain.Enums;

namespace MealMate.Application.Services;

public interface IFridgeDbContext
{
    DbSet<FridgeItem> FridgeItems { get; }
    DbSet<Household> Households { get; }
    DbSet<HouseholdMember> HouseholdMembers { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public class FridgeService : IFridgeService
{
    private readonly IFridgeDbContext _context;
    private readonly IMapper _mapper;
    private readonly ILogger<FridgeService> _logger;

    public FridgeService(IFridgeDbContext context, IMapper mapper, ILogger<FridgeService> logger)
    {
        _context = context;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ServiceResult<IEnumerable<FridgeItemDto>>> GetByHouseholdAsync(Guid householdId, Guid userId)
    {
        if (!await IsMemberAsync(householdId, userId))
            return ServiceResult<IEnumerable<FridgeItemDto>>.Forbidden("You are not a member of this household.");

        var items = await _context.FridgeItems
            .Include(f => f.Ingredient)
            .Include(f => f.AddedByUser)
            .Where(f => f.HouseholdId == householdId)
            .OrderBy(f => f.ExpiryDate)
            .ThenBy(f => f.Ingredient.Name)
            .ToListAsync();

        return ServiceResult<IEnumerable<FridgeItemDto>>.Ok(_mapper.Map<IEnumerable<FridgeItemDto>>(items));
    }

    public async Task<ServiceResult<FridgeItemDto>> AddItemAsync(Guid householdId, CreateFridgeItemDto dto, Guid userId)
    {
        if (!await IsMemberAsync(householdId, userId))
            return ServiceResult<FridgeItemDto>.Forbidden("You are not a member of this household.");

        var item = new FridgeItem
        {
            Id = Guid.NewGuid(),
            HouseholdId = householdId,
            IngredientId = dto.IngredientId,
            Quantity = dto.Quantity,
            Unit = dto.Unit,
            ExpiryDate = dto.ExpiryDate,
            AddedByUserId = userId,
            AddedAt = DateTime.UtcNow,
            Source = FridgeItemSource.Manual
        };

        _context.FridgeItems.Add(item);
        await _context.SaveChangesAsync();

        var created = await _context.FridgeItems
            .Include(f => f.Ingredient)
            .Include(f => f.AddedByUser)
            .FirstAsync(f => f.Id == item.Id);

        return ServiceResult<FridgeItemDto>.Ok(_mapper.Map<FridgeItemDto>(created));
    }

    public async Task<ServiceResult<FridgeItemDto>> UpdateItemAsync(Guid itemId, UpdateFridgeItemDto dto, Guid userId)
    {
        var item = await _context.FridgeItems
            .Include(f => f.Ingredient)
            .Include(f => f.AddedByUser)
            .FirstOrDefaultAsync(f => f.Id == itemId);

        if (item == null)
            return ServiceResult<FridgeItemDto>.NotFound("Fridge item not found.");

        if (!await IsMemberAsync(item.HouseholdId, userId))
            return ServiceResult<FridgeItemDto>.Forbidden("You are not a member of this household.");

        item.Quantity = dto.Quantity;
        item.Unit = dto.Unit;
        item.ExpiryDate = dto.ExpiryDate;

        await _context.SaveChangesAsync();

        return ServiceResult<FridgeItemDto>.Ok(_mapper.Map<FridgeItemDto>(item));
    }

    public async Task<ServiceResult<bool>> DeleteItemAsync(Guid itemId, Guid userId)
    {
        var item = await _context.FridgeItems.FindAsync(itemId);
        if (item == null)
            return ServiceResult<bool>.NotFound("Fridge item not found.");

        if (!await IsMemberAsync(item.HouseholdId, userId))
            return ServiceResult<bool>.Forbidden("You are not a member of this household.");

        _context.FridgeItems.Remove(item);
        await _context.SaveChangesAsync();

        return ServiceResult<bool>.Ok(true);
    }

    private async Task<bool> IsMemberAsync(Guid householdId, Guid userId)
    {
        return await _context.HouseholdMembers
            .AnyAsync(m => m.HouseholdId == householdId && m.UserId == userId);
    }
}
