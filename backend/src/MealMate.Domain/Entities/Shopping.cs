using MealMate.Domain.Enums;

namespace MealMate.Domain.Entities;

public class ShoppingList
{
    public Guid Id { get; set; }
    public Guid HouseholdId { get; set; }
    public Household Household { get; set; } = null!;
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public bool IsCompleted { get; set; } = false;
    public DateTime? CompletedAt { get; set; }

    public ICollection<ShoppingListItem> Items { get; set; } = new List<ShoppingListItem>();
}

public class ShoppingListItem
{
    public Guid Id { get; set; }
    public Guid ShoppingListId { get; set; }
    public ShoppingList ShoppingList { get; set; } = null!;
    public Guid IngredientId { get; set; }
    public Ingredient Ingredient { get; set; } = null!;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public bool IsBought { get; set; } = false;
    public Guid? BoughtByUserId { get; set; }
    public ApplicationUser? BoughtByUser { get; set; }
    public DateTime? BoughtAt { get; set; }
    public ShoppingItemSource Source { get; set; } = ShoppingItemSource.Manual;
}
