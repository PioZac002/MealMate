using MealMate.Domain.Enums;

namespace MealMate.Domain.Entities;

public class FridgeItem
{
    public Guid Id { get; set; }
    public Guid HouseholdId { get; set; }
    public Household Household { get; set; } = null!;
    public Guid IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public DateTime? ExpiryDate { get; set; }
    public Guid AddedByUserId { get; set; }
    public ApplicationUser AddedByUser { get; set; } = null!;
    public DateTime AddedAt { get; set; } = DateTime.UtcNow;
    public FridgeItemSource Source { get; set; } = FridgeItemSource.Manual;
}

public class MustHaveProduct
{
    public Guid Id { get; set; }
    public Guid IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;
    public Guid HouseholdId { get; set; }
    public Household Household { get; set; } = null!;
    public decimal MinQuantity { get; set; }
    public string Unit { get; set; } = string.Empty;
}
