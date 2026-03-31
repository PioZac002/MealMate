using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MealMate.Application.DTOs.Fitness;
using MealMate.Application.Interfaces;

namespace MealMate.API.Controllers;

[ApiController]
[Route("api/workouts")]
[Authorize]
public class WorkoutsController : ControllerBase
{
    private readonly IFitnessService _fitnessService;

    public WorkoutsController(IFitnessService fitnessService)
    {
        _fitnessService = fitnessService;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Get workout history for the last N days</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<WorkoutDto>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] int days = 30)
    {
        var result = await _fitnessService.GetWorkoutsAsync(CurrentUserId, days);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return Ok(result.Data);
    }

    /// <summary>Get a specific workout with all sets</summary>
    [HttpGet("{workoutId:guid}")]
    [ProducesResponseType(typeof(WorkoutDetailDto), 200)]
    public async Task<IActionResult> GetById(Guid workoutId)
    {
        var result = await _fitnessService.GetWorkoutAsync(workoutId, CurrentUserId);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return Ok(result.Data);
    }

    /// <summary>Log a new workout session</summary>
    [HttpPost]
    [ProducesResponseType(typeof(WorkoutDetailDto), 201)]
    public async Task<IActionResult> Log([FromBody] LogWorkoutDto dto)
    {
        var result = await _fitnessService.LogWorkoutAsync(dto, CurrentUserId);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return StatusCode(201, result.Data);
    }

    /// <summary>Delete a logged workout</summary>
    [HttpDelete("{workoutId:guid}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(Guid workoutId)
    {
        var result = await _fitnessService.DeleteWorkoutAsync(workoutId, CurrentUserId);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return NoContent();
    }
}
