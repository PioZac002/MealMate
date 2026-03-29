using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using MealMate.Application.Common;
using MealMate.Application.DTOs.Household;
using MealMate.Application.Interfaces;
using MealMate.Domain.Entities;
using MealMate.Domain.Enums;

namespace MealMate.Application.Services;

public interface IApplicationDbContext
{
    Microsoft.EntityFrameworkCore.DbSet<Household> Households { get; }
    Microsoft.EntityFrameworkCore.DbSet<HouseholdMember> HouseholdMembers { get; }
    Microsoft.EntityFrameworkCore.DbSet<InviteCode> InviteCodes { get; }
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}

public class HouseholdService : IHouseholdService
{
    private readonly IApplicationDbContext _context;
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IEmailService _emailService;
    private readonly IMapper _mapper;
    private readonly ILogger<HouseholdService> _logger;

    public HouseholdService(
        IApplicationDbContext context,
        UserManager<ApplicationUser> userManager,
        IEmailService emailService,
        IMapper mapper,
        ILogger<HouseholdService> logger)
    {
        _context = context;
        _userManager = userManager;
        _emailService = emailService;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ServiceResult<HouseholdDetailDto>> GetByIdAsync(Guid id, Guid userId)
    {
        var household = await _context.Households
            .Include(h => h.CreatedByUser)
            .Include(h => h.Members).ThenInclude(m => m.User)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (household == null)
            return ServiceResult<HouseholdDetailDto>.NotFound("Household not found.");

        var isMember = household.Members.Any(m => m.UserId == userId);
        if (!isMember)
            return ServiceResult<HouseholdDetailDto>.Forbidden("You are not a member of this household.");

        return ServiceResult<HouseholdDetailDto>.Ok(_mapper.Map<HouseholdDetailDto>(household));
    }

    public async Task<ServiceResult<IEnumerable<HouseholdDto>>> GetMyHouseholdsAsync(Guid userId)
    {
        var households = await _context.Households
            .Include(h => h.CreatedByUser)
            .Include(h => h.Members)
            .Where(h => h.Members.Any(m => m.UserId == userId))
            .ToListAsync();

        return ServiceResult<IEnumerable<HouseholdDto>>.Ok(
            _mapper.Map<IEnumerable<HouseholdDto>>(households));
    }

    public async Task<ServiceResult<HouseholdDto>> CreateAsync(CreateHouseholdDto dto, Guid userId)
    {
        var household = new Household
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        var member = new HouseholdMember
        {
            UserId = userId,
            HouseholdId = household.Id,
            Role = HouseholdRole.Admin,
            JoinedAt = DateTime.UtcNow
        };

        household.Members.Add(member);
        _context.Households.Add(household);
        await _context.SaveChangesAsync();

        var result = await _context.Households
            .Include(h => h.CreatedByUser)
            .Include(h => h.Members)
            .FirstAsync(h => h.Id == household.Id);

        return ServiceResult<HouseholdDto>.Created(_mapper.Map<HouseholdDto>(result));
    }

    public async Task<ServiceResult<HouseholdDto>> UpdateAsync(Guid id, UpdateHouseholdDto dto, Guid userId)
    {
        var household = await _context.Households
            .Include(h => h.Members)
            .Include(h => h.CreatedByUser)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (household == null)
            return ServiceResult<HouseholdDto>.NotFound("Household not found.");

        var member = household.Members.FirstOrDefault(m => m.UserId == userId);
        if (member == null || member.Role != HouseholdRole.Admin)
            return ServiceResult<HouseholdDto>.Forbidden("Only admin can update the household.");

        household.Name = dto.Name;
        await _context.SaveChangesAsync();
        return ServiceResult<HouseholdDto>.Ok(_mapper.Map<HouseholdDto>(household));
    }

    public async Task<ServiceResult<bool>> DeleteAsync(Guid id, Guid userId)
    {
        var household = await _context.Households
            .Include(h => h.Members)
            .FirstOrDefaultAsync(h => h.Id == id);

        if (household == null)
            return ServiceResult<bool>.NotFound("Household not found.");

        if (household.CreatedByUserId != userId)
            return ServiceResult<bool>.Forbidden("Only the creator can delete the household.");

        _context.Households.Remove(household);
        await _context.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }

    public async Task<ServiceResult<InviteCodeDto>> InviteMemberAsync(Guid householdId, InviteMemberDto dto, Guid userId)
    {
        var household = await _context.Households
            .Include(h => h.Members)
            .Include(h => h.CreatedByUser)
            .FirstOrDefaultAsync(h => h.Id == householdId);

        if (household == null)
            return ServiceResult<InviteCodeDto>.NotFound("Household not found.");

        var member = household.Members.FirstOrDefault(m => m.UserId == userId);
        if (member == null || member.Role != HouseholdRole.Admin)
            return ServiceResult<InviteCodeDto>.Forbidden("Only admin can invite members.");

        var alreadyMember = await _userManager.FindByEmailAsync(dto.Email);
        if (alreadyMember != null && household.Members.Any(m => m.UserId == alreadyMember.Id))
            return ServiceResult<InviteCodeDto>.Fail("User is already a member of this household.");

        var code = GenerateCode();
        var inviteCode = new InviteCode
        {
            Id = Guid.NewGuid(),
            Code = code,
            HouseholdId = householdId,
            InvitedEmail = dto.Email.ToLower(),
            ExpiresAt = DateTime.UtcNow.AddDays(7),
            IsUsed = false,
            CreatedByUserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        _context.InviteCodes.Add(inviteCode);
        await _context.SaveChangesAsync();

        var inviter = await _userManager.FindByIdAsync(userId.ToString());
        var inviterName = inviter != null ? $"{inviter.FirstName} {inviter.LastName}" : "A household admin";

        try
        {
            await _emailService.SendInviteCodeAsync(dto.Email, household.Name, code, inviterName);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send invite email to {Email}", dto.Email);
        }

        var result = new InviteCodeDto
        {
            Id = inviteCode.Id,
            Code = inviteCode.Code,
            HouseholdId = inviteCode.HouseholdId,
            HouseholdName = household.Name,
            InvitedEmail = inviteCode.InvitedEmail,
            ExpiresAt = inviteCode.ExpiresAt,
            IsUsed = inviteCode.IsUsed,
            CreatedAt = inviteCode.CreatedAt
        };

        return ServiceResult<InviteCodeDto>.Created(result);
    }

    public async Task<ServiceResult<HouseholdDto>> JoinHouseholdAsync(JoinHouseholdDto dto, Guid userId, string userEmail)
    {
        var invite = await _context.InviteCodes
            .Include(i => i.Household).ThenInclude(h => h.Members)
            .Include(i => i.Household).ThenInclude(h => h.CreatedByUser)
            .FirstOrDefaultAsync(i => i.Code == dto.Code);

        if (invite == null)
            return ServiceResult<HouseholdDto>.NotFound("Invite code not found.");

        if (invite.IsUsed)
            return ServiceResult<HouseholdDto>.Fail("Invite code has already been used.");

        if (invite.ExpiresAt < DateTime.UtcNow)
            return ServiceResult<HouseholdDto>.Fail("Invite code has expired.");

        if (!string.Equals(invite.InvitedEmail, userEmail, StringComparison.OrdinalIgnoreCase))
            return ServiceResult<HouseholdDto>.Forbidden("This invite code is not for your email address.");

        if (invite.Household.Members.Any(m => m.UserId == userId))
            return ServiceResult<HouseholdDto>.Fail("You are already a member of this household.");

        var member = new HouseholdMember
        {
            UserId = userId,
            HouseholdId = invite.HouseholdId,
            Role = HouseholdRole.Member,
            JoinedAt = DateTime.UtcNow
        };

        invite.Household.Members.Add(member);
        invite.IsUsed = true;
        await _context.SaveChangesAsync();

        return ServiceResult<HouseholdDto>.Ok(_mapper.Map<HouseholdDto>(invite.Household));
    }

    public async Task<ServiceResult<bool>> RemoveMemberAsync(Guid householdId, Guid memberId, Guid userId)
    {
        var household = await _context.Households
            .Include(h => h.Members)
            .FirstOrDefaultAsync(h => h.Id == householdId);

        if (household == null)
            return ServiceResult<bool>.NotFound("Household not found.");

        var requester = household.Members.FirstOrDefault(m => m.UserId == userId);
        if (requester == null || requester.Role != HouseholdRole.Admin)
            return ServiceResult<bool>.Forbidden("Only admin can remove members.");

        if (memberId == userId)
            return ServiceResult<bool>.Fail("Admin cannot remove themselves.");

        var target = household.Members.FirstOrDefault(m => m.UserId == memberId);
        if (target == null)
            return ServiceResult<bool>.NotFound("Member not found in household.");

        _context.HouseholdMembers.Remove(target);
        await _context.SaveChangesAsync();
        return ServiceResult<bool>.Ok(true);
    }

    private static string GenerateCode()
    {
        const string chars = "ABCDEFGHJKLMNPQRSTUVWXYZ23456789";
        var random = new Random();
        return new string(Enumerable.Repeat(chars, 5).Select(s => s[random.Next(s.Length)]).ToArray());
    }
}
