using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MealMate.Application.DTOs.Household;
using MealMate.Application.Interfaces;

namespace MealMate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class HouseholdsController : ControllerBase
{
    private readonly IHouseholdService _householdService;

    public HouseholdsController(IHouseholdService householdService)
    {
        _householdService = householdService;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
    private string CurrentUserEmail => User.FindFirstValue(ClaimTypes.Email)!;

    /// <summary>Get all households for the current user</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<HouseholdDto>), 200)]
    public async Task<IActionResult> GetMyHouseholds()
    {
        var result = await _householdService.GetMyHouseholdsAsync(CurrentUserId);
        return Ok(result.Data);
    }

    /// <summary>Get household details by ID</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(HouseholdDetailDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _householdService.GetByIdAsync(id, CurrentUserId);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return Ok(result.Data);
    }

    /// <summary>Create a new household</summary>
    [HttpPost]
    [ProducesResponseType(typeof(HouseholdDto), 201)]
    public async Task<IActionResult> Create([FromBody] CreateHouseholdDto dto)
    {
        var result = await _householdService.CreateAsync(dto, CurrentUserId);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    /// <summary>Update household name (admin only)</summary>
    [HttpPut("{id:guid}")]
    [ProducesResponseType(typeof(HouseholdDto), 200)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateHouseholdDto dto)
    {
        var result = await _householdService.UpdateAsync(id, dto, CurrentUserId);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return Ok(result.Data);
    }

    /// <summary>Delete a household (creator only)</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _householdService.DeleteAsync(id, CurrentUserId);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return NoContent();
    }

    /// <summary>Invite a member by email (admin only)</summary>
    [HttpPost("{id:guid}/invite")]
    [ProducesResponseType(typeof(InviteCodeDto), 201)]
    public async Task<IActionResult> InviteMember(Guid id, [FromBody] InviteMemberDto dto)
    {
        var result = await _householdService.InviteMemberAsync(id, dto, CurrentUserId);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return StatusCode(201, result.Data);
    }

    /// <summary>Join a household using an invite code</summary>
    [HttpPost("join")]
    [ProducesResponseType(typeof(HouseholdDto), 200)]
    public async Task<IActionResult> Join([FromBody] JoinHouseholdDto dto)
    {
        var result = await _householdService.JoinHouseholdAsync(dto, CurrentUserId, CurrentUserEmail);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return Ok(result.Data);
    }

    /// <summary>Remove a member from household (admin only)</summary>
    [HttpDelete("{householdId:guid}/members/{memberId:guid}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> RemoveMember(Guid householdId, Guid memberId)
    {
        var result = await _householdService.RemoveMemberAsync(householdId, memberId, CurrentUserId);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return NoContent();
    }
}
