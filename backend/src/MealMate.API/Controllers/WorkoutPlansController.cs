using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MealMate.Application.DTOs.Fitness;
using MealMate.Application.Interfaces;

namespace MealMate.API.Controllers;

[ApiController]
[Route("api/workout-plans")]
[Authorize]
public class WorkoutPlansController : ControllerBase
{
    private readonly IFitnessService _fitnessService;

    public WorkoutPlansController(IFitnessService fitnessService)
    {
        _fitnessService = fitnessService;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Get all workout plans for current user</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<WorkoutPlanDto>), 200)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _fitnessService.GetWorkoutPlansAsync(CurrentUserId);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return Ok(result.Data);
    }

    /// <summary>Get a specific workout plan with exercises</summary>
    [HttpGet("{planId:guid}")]
    [ProducesResponseType(typeof(WorkoutPlanDetailDto), 200)]
    public async Task<IActionResult> GetById(Guid planId)
    {
        var result = await _fitnessService.GetWorkoutPlanAsync(planId, CurrentUserId);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return Ok(result.Data);
    }

    /// <summary>Create a new workout plan</summary>
    [HttpPost]
    [ProducesResponseType(typeof(WorkoutPlanDetailDto), 201)]
    public async Task<IActionResult> Create([FromBody] CreateWorkoutPlanDto dto)
    {
        var result = await _fitnessService.CreateWorkoutPlanAsync(dto, CurrentUserId);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return StatusCode(201, result.Data);
    }

    /// <summary>Delete a workout plan</summary>
    [HttpDelete("{planId:guid}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(Guid planId)
    {
        var result = await _fitnessService.DeleteWorkoutPlanAsync(planId, CurrentUserId);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return NoContent();
    }
}
