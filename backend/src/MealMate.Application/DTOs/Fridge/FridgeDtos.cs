namespace MealMate.Application.DTOs.Fridge;

public class FridgeItemDto
{
    public Guid Id { get; set; }
    public Guid HouseholdId { get; set; }
    public Guid IngredientId { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public string IngredientCategory { get; set; } = string.Empty;
    public string? IngredientImageUrl { get; set; }
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public DateTime? ExpiryDate { get; set; }
    public DateTime AddedAt { get; set; }
    public string Source { get; set; } = string.Empty;
    public string AddedByUserName { get; set; } = string.Empty;
    public bool IsExpiringSoon => ExpiryDate.HasValue && ExpiryDate.Value <= DateTime.UtcNow.AddDays(3);
    public bool IsExpired => ExpiryDate.HasValue && ExpiryDate.Value < DateTime.UtcNow;
}

public class CreateFridgeItemDto
{
    public Guid IngredientId { get; set; }
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public DateTime? ExpiryDate { get; set; }
}

public class UpdateFridgeItemDto
{
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public DateTime? ExpiryDate { get; set; }
}
