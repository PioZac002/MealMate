using MealMate.Application.Common;
using MealMate.Application.DTOs.Household;

namespace MealMate.Application.Interfaces;

public interface IHouseholdService
{
    Task<ServiceResult<HouseholdDetailDto>> GetByIdAsync(Guid id, Guid userId);
    Task<ServiceResult<IEnumerable<HouseholdDto>>> GetMyHouseholdsAsync(Guid userId);
    Task<ServiceResult<HouseholdDto>> CreateAsync(CreateHouseholdDto dto, Guid userId);
    Task<ServiceResult<HouseholdDto>> UpdateAsync(Guid id, UpdateHouseholdDto dto, Guid userId);
    Task<ServiceResult<bool>> DeleteAsync(Guid id, Guid userId);
    Task<ServiceResult<InviteCodeDto>> InviteMemberAsync(Guid householdId, InviteMemberDto dto, Guid userId);
    Task<ServiceResult<HouseholdDto>> JoinHouseholdAsync(JoinHouseholdDto dto, Guid userId, string userEmail);
    Task<ServiceResult<bool>> RemoveMemberAsync(Guid householdId, Guid memberId, Guid userId);
}
