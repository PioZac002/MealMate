namespace MealMate.Application.DTOs.Shopping;

public class ShoppingListDto
{
    public Guid Id { get; set; }
    public Guid HouseholdId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int ItemCount { get; set; }
    public int BoughtCount { get; set; }
}

public class ShoppingListDetailDto
{
    public Guid Id { get; set; }
    public Guid HouseholdId { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsCompleted { get; set; }
    public DateTime? CompletedAt { get; set; }
    public IEnumerable<ShoppingListItemDto> Items { get; set; } = new List<ShoppingListItemDto>();
}

public class ShoppingListItemDto
{
    public Guid Id { get; set; }
    public Guid ShoppingListId { get; set; }
    public Guid IngredientId { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public string? IngredientImageUrl { get; set; }
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public bool IsBought { get; set; }
    public DateTime? BoughtAt { get; set; }
    public string Source { get; set; } = string.Empty;
}

public class CreateShoppingListDto
{
    public string Name { get; set; } = string.Empty;
}

public class AddShoppingListItemDto
{
    public Guid IngredientId { get; set; }
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
}
