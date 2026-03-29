using MealMate.Domain.Enums;

namespace MealMate.Application.DTOs.Ingredient;

public class CreateIngredientDto
{
    public string Name { get; set; } = string.Empty;
    public string DefaultUnit { get; set; } = string.Empty;
    public IngredientCategory Category { get; set; }
    public decimal CaloriesPer100g { get; set; }
    public decimal ProteinPer100g { get; set; }
    public decimal CarbsPer100g { get; set; }
    public decimal FatPer100g { get; set; }
    public string? ImageUrl { get; set; }
}

public class UpdateIngredientDto : CreateIngredientDto { }

public class IngredientDto
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
}

public class IngredientFilterDto
{
    public string? Search { get; set; }
    public IngredientCategory? Category { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
