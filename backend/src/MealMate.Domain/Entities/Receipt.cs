namespace MealMate.Domain.Entities;

public class Receipt
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public Guid HouseholdId { get; set; }
    public Household Household { get; set; } = null!;
    public string? ImageUrl { get; set; }
    public DateTime ScannedAt { get; set; } = DateTime.UtcNow;
    public string? StoreName { get; set; }
    public decimal? TotalAmount { get; set; }

    public ICollection<ReceiptItem> Items { get; set; } = new List<ReceiptItem>();
}

public class ReceiptItem
{
    public Guid Id { get; set; }
    public Guid ReceiptId { get; set; }
    public Receipt Receipt { get; set; } = null!;
    public string RawText { get; set; } = string.Empty;
    public Guid? IngredientId { get; set; }
    public Ingredient? Ingredient { get; set; }
    public decimal Quantity { get; set; }
    public decimal Price { get; set; }
    public bool IsMatched { get; set; } = false;
    public bool IsManuallyCorrected { get; set; } = false;
}
