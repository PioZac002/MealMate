using MealMate.Domain.Enums;

namespace MealMate.Domain.Entities;

public class DailyNutritionLog
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public DateOnly Date { get; set; }
    public decimal CalorieGoal { get; set; }
    public decimal ProteinGoal { get; set; }
    public decimal CarbsGoal { get; set; }
    public decimal FatGoal { get; set; }
    public string? Notes { get; set; }

    public ICollection<MealLog> MealLogs { get; set; } = new List<MealLog>();
}

public class MealLog
{
    public Guid Id { get; set; }
    public Guid DailyNutritionLogId { get; set; }
    public DailyNutritionLog DailyNutritionLog { get; set; } = null!;
    public MealType MealType { get; set; }
    public Guid? RecipeId { get; set; }
    public Recipe? Recipe { get; set; }
    public string? CustomFoodName { get; set; }
    public decimal Calories { get; set; }
    public decimal Protein { get; set; }
    public decimal Carbs { get; set; }
    public decimal Fat { get; set; }
    public decimal Servings { get; set; }
    public DateTime LoggedAt { get; set; } = DateTime.UtcNow;
}
