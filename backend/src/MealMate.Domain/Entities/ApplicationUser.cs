using Microsoft.AspNetCore.Identity;

namespace MealMate.Domain.Entities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public string? RefreshToken { get; set; }
    public DateTime? RefreshTokenExpiryTime { get; set; }

    public ICollection<HouseholdMember> HouseholdMemberships { get; set; } = new List<HouseholdMember>();
    public ICollection<Household> CreatedHouseholds { get; set; } = new List<Household>();
    public ICollection<Recipe> Recipes { get; set; } = new List<Recipe>();
    public ICollection<InviteCode> CreatedInvites { get; set; } = new List<InviteCode>();
    public ICollection<FridgeItem> FridgeItems { get; set; } = new List<FridgeItem>();
    public ICollection<DailyNutritionLog> NutritionLogs { get; set; } = new List<DailyNutritionLog>();
    public ICollection<Workout> Workouts { get; set; } = new List<Workout>();
    public ICollection<WorkoutPlan> WorkoutPlans { get; set; } = new List<WorkoutPlan>();
    public ICollection<Receipt> Receipts { get; set; } = new List<Receipt>();
}
