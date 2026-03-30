using MealMate.Domain.Enums;

namespace MealMate.Application.DTOs.Household;

public class CreateHouseholdDto
{
    public string Name { get; set; } = string.Empty;
}

public class UpdateHouseholdDto
{
    public string Name { get; set; } = string.Empty;
}

public class HouseholdDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public Guid CreatedByUserId { get; set; }
    public string CreatedByUserName { get; set; } = string.Empty;
    public int MemberCount { get; set; }
}

public class HouseholdDetailDto : HouseholdDto
{
    public IEnumerable<HouseholdMemberDto> Members { get; set; } = Enumerable.Empty<HouseholdMemberDto>();
}

public class HouseholdMemberDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = string.Empty;
    public string FullName { get; set; } = string.Empty;
    public string? AvatarUrl { get; set; }
    public HouseholdRole Role { get; set; }
    public DateTime JoinedAt { get; set; }
}

public class InviteMemberDto
{
    public string Email { get; set; } = string.Empty;
}

public class JoinHouseholdDto
{
    public string Code { get; set; } = string.Empty;
}

public class InviteCodeDto
{
    public Guid Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public Guid HouseholdId { get; set; }
    public string HouseholdName { get; set; } = string.Empty;
    public string InvitedEmail { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public bool IsUsed { get; set; }
    public DateTime CreatedAt { get; set; }
}
