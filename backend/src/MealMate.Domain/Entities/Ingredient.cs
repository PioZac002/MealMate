using MealMate.Domain.Enums;

namespace MealMate.Domain.Entities;

public class Ingredient
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string DefaultUnit { get; set; } = string.Empty;
    public IngredientCategory Category { get; set; }
    public decimal CaloriesPer100g { get; set; }
    public decimal ProteinPer100g { get; set; }
    public decimal CarbsPer100g { get; set; }
    public decimal FatPer100g { get; set; }
    public string? ImageUrl { get; set; }

    public ICollection<RecipeIngredient> RecipeIngredients { get; set; } = new List<RecipeIngredient>();
    public ICollection<FridgeItem> FridgeItems { get; set; } = new List<FridgeItem>();
    public ICollection<ShoppingListItem> ShoppingListItems { get; set; } = new List<ShoppingListItem>();
    public ICollection<MustHaveProduct> MustHaveProducts { get; set; } = new List<MustHaveProduct>();
}
