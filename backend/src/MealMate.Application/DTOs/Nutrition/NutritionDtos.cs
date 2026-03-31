namespace MealMate.Application.DTOs.Nutrition;

public class DailyNutritionLogDto
{
    public Guid Id { get; set; }
    public DateOnly Date { get; set; }
    public decimal CalorieGoal { get; set; }
    public decimal ProteinGoal { get; set; }
    public decimal CarbsGoal { get; set; }
    public decimal FatGoal { get; set; }
    public string? Notes { get; set; }
    public IEnumerable<MealLogDto> MealLogs { get; set; } = Enumerable.Empty<MealLogDto>();
    public decimal TotalCalories => MealLogs.Sum(m => m.Calories);
    public decimal TotalProtein => MealLogs.Sum(m => m.Protein);
    public decimal TotalCarbs => MealLogs.Sum(m => m.Carbs);
    public decimal TotalFat => MealLogs.Sum(m => m.Fat);
}

public class MealLogDto
{
    public Guid Id { get; set; }
    public string MealType { get; set; } = string.Empty;
    public Guid? RecipeId { get; set; }
    public string? RecipeName { get; set; }
    public string? CustomFoodName { get; set; }
    public decimal Calories { get; set; }
    public decimal Protein { get; set; }
    public decimal Carbs { get; set; }
    public decimal Fat { get; set; }
    public decimal Servings { get; set; }
    public DateTime LoggedAt { get; set; }
    public string FoodName => RecipeName ?? CustomFoodName ?? "Custom";
}

public class SetGoalsDto
{
    public decimal CalorieGoal { get; set; }
    public decimal ProteinGoal { get; set; }
    public decimal CarbsGoal { get; set; }
    public decimal FatGoal { get; set; }
    public string? Notes { get; set; }
}

public class AddMealLogDto
{
    public string MealType { get; set; } = "Snack";
    public Guid? RecipeId { get; set; }
    public string? CustomFoodName { get; set; }
    public decimal Calories { get; set; }
    public decimal Protein { get; set; }
    public decimal Carbs { get; set; }
    public decimal Fat { get; set; }
    public decimal Servings { get; set; } = 1;
}
