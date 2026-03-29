using MealMate.Domain.Enums;

namespace MealMate.Domain.Entities;

public class MealPlan
{
    public Guid Id { get; set; }
    public Guid HouseholdId { get; set; }
    public Household Household { get; set; } = null!;
    public DateTime WeekStartDate { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<MealPlanEntry> Entries { get; set; } = new List<MealPlanEntry>();
}

public class MealPlanEntry
{
    public Guid Id { get; set; }
    public Guid MealPlanId { get; set; }
    public MealPlan MealPlan { get; set; } = null!;
    public DayOfWeekEnum DayOfWeek { get; set; }
    public MealType MealType { get; set; }
    public Guid RecipeId { get; set; }
    public Recipe Recipe { get; set; } = null!;
    public int Servings { get; set; }
    public string? Notes { get; set; }
}
