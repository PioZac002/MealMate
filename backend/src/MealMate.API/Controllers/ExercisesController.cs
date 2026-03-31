using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MealMate.Application.DTOs.Fitness;
using MealMate.Application.Interfaces;

namespace MealMate.API.Controllers;

[ApiController]
[Route("api/exercises")]
[Authorize]
public class ExercisesController : ControllerBase
{
    private readonly IFitnessService _fitnessService;

    public ExercisesController(IFitnessService fitnessService)
    {
        _fitnessService = fitnessService;
    }

    /// <summary>Get all exercises with optional filtering</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ExerciseDto>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] string? search, [FromQuery] string? muscleGroup)
    {
        var result = await _fitnessService.GetExercisesAsync(search, muscleGroup);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return Ok(result.Data);
    }

    /// <summary>Create a new exercise</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ExerciseDto), 201)]
    public async Task<IActionResult> Create([FromBody] CreateExerciseDto dto)
    {
        var result = await _fitnessService.CreateExerciseAsync(dto);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return StatusCode(201, result.Data);
    }
}
