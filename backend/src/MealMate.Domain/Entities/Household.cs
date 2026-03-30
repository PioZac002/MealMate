using MealMate.Domain.Enums;

namespace MealMate.Domain.Entities;

public class Household
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public Guid CreatedByUserId { get; set; }
    public ApplicationUser CreatedByUser { get; set; } = null!;

    public ICollection<HouseholdMember> Members { get; set; } = new List<HouseholdMember>();
    public ICollection<InviteCode> InviteCodes { get; set; } = new List<InviteCode>();
    public ICollection<MealPlan> MealPlans { get; set; } = new List<MealPlan>();
    public ICollection<FridgeItem> FridgeItems { get; set; } = new List<FridgeItem>();
    public ICollection<MustHaveProduct> MustHaveProducts { get; set; } = new List<MustHaveProduct>();
    public ICollection<ShoppingList> ShoppingLists { get; set; } = new List<ShoppingList>();
    public ICollection<Receipt> Receipts { get; set; } = new List<Receipt>();
}

public class HouseholdMember
{
    public Guid UserId { get; set; }
    public ApplicationUser User { get; set; } = null!;
    public Guid HouseholdId { get; set; }
    public Household Household { get; set; } = null!;
    public HouseholdRole Role { get; set; } = HouseholdRole.Member;
    public DateTime JoinedAt { get; set; } = DateTime.UtcNow;
}

public class InviteCode
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public Guid HouseholdId { get; set; }
    public Household Household { get; set; } = null!;
    public string InvitedEmail { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; } = false;
    public Guid CreatedByUserId { get; set; }
    public ApplicationUser CreatedByUser { get; set; } = null!;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
