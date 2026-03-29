using MealMate.Domain.Enums;

namespace MealMate.Application.DTOs.Recipe;

public class CreateRecipeDto
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int PrepTimeMinutes { get; set; }
    public int CookTimeMinutes { get; set; }
    public int Servings { get; set; }
    public DietType DietType { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsPublic { get; set; } = true;
    public IEnumerable<CreateRecipeIngredientDto> Ingredients { get; set; } = Enumerable.Empty<CreateRecipeIngredientDto>();
    public IEnumerable<CreateRecipeStepDto> Steps { get; set; } = Enumerable.Empty<CreateRecipeStepDto>();
}

public class UpdateRecipeDto : CreateRecipeDto { }

public class CreateRecipeIngredientDto
{
    public Guid IngredientId { get; set; }
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
}

public class CreateRecipeStepDto
{
    public int StepNumber { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}

public class RecipeDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public int PrepTimeMinutes { get; set; }
    public int CookTimeMinutes { get; set; }
    public int Servings { get; set; }
    public DietType DietType { get; set; }
    public string? ImageUrl { get; set; }
    public bool IsPublic { get; set; }
    public DateTime CreatedAt { get; set; }
    public Guid CreatedByUserId { get; set; }
    public string CreatedByUserName { get; set; } = string.Empty;
}

public class RecipeDetailDto : RecipeDto
{
    public IEnumerable<RecipeIngredientDto> Ingredients { get; set; } = Enumerable.Empty<RecipeIngredientDto>();
    public IEnumerable<RecipeStepDto> Steps { get; set; } = Enumerable.Empty<RecipeStepDto>();
    public decimal TotalCalories { get; set; }
    public decimal TotalProtein { get; set; }
    public decimal TotalCarbs { get; set; }
    public decimal TotalFat { get; set; }
}

public class RecipeIngredientDto
{
    public Guid Id { get; set; }
    public Guid IngredientId { get; set; }
    public string IngredientName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public string Unit { get; set; } = string.Empty;
    public decimal CaloriesPer100g { get; set; }
}

public class RecipeStepDto
{
    public Guid Id { get; set; }
    public int StepNumber { get; set; }
    public string Description { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }
}

public class RecipeFilterDto
{
    public string? Search { get; set; }
    public DietType? DietType { get; set; }
    public int? MaxPrepTime { get; set; }
    public int? MaxCookTime { get; set; }
    public bool? IsPublic { get; set; }
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}
