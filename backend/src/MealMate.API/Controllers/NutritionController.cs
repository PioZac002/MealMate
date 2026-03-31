using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MealMate.Application.DTOs.Nutrition;
using MealMate.Application.Interfaces;

namespace MealMate.API.Controllers;

[ApiController]
[Route("api/nutrition")]
[Authorize]
public class NutritionController : ControllerBase
{
    private readonly INutritionService _nutritionService;

    public NutritionController(INutritionService nutritionService)
    {
        _nutritionService = nutritionService;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Get nutrition log for today</summary>
    [HttpGet("today")]
    [ProducesResponseType(typeof(DailyNutritionLogDto), 200)]
    public async Task<IActionResult> GetToday()
    {
        var date = DateOnly.FromDateTime(DateTime.UtcNow);
        var result = await _nutritionService.GetOrCreateLogAsync(date, CurrentUserId);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return Ok(result.Data);
    }

    /// <summary>Get nutrition log for a specific date (yyyy-MM-dd)</summary>
    [HttpGet("{date}")]
    [ProducesResponseType(typeof(DailyNutritionLogDto), 200)]
    public async Task<IActionResult> GetByDate(string date)
    {
        if (!DateOnly.TryParseExact(date, "yyyy-MM-dd", out var parsedDate))
            return BadRequest(new { error = "Date must be in yyyy-MM-dd format" });

        var result = await _nutritionService.GetOrCreateLogAsync(parsedDate, CurrentUserId);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return Ok(result.Data);
    }

    /// <summary>Set nutrition goals for a date</summary>
    [HttpPut("{date}/goals")]
    [ProducesResponseType(typeof(DailyNutritionLogDto), 200)]
    public async Task<IActionResult> SetGoals(string date, [FromBody] SetGoalsDto dto)
    {
        if (!DateOnly.TryParseExact(date, "yyyy-MM-dd", out var parsedDate))
            return BadRequest(new { error = "Date must be in yyyy-MM-dd format" });

        var result = await _nutritionService.SetGoalsAsync(parsedDate, dto, CurrentUserId);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return Ok(result.Data);
    }

    /// <summary>Add a meal log entry for a date</summary>
    [HttpPost("{date}/meals")]
    [ProducesResponseType(typeof(MealLogDto), 201)]
    public async Task<IActionResult> AddMeal(string date, [FromBody] AddMealLogDto dto)
    {
        if (!DateOnly.TryParseExact(date, "yyyy-MM-dd", out var parsedDate))
            return BadRequest(new { error = "Date must be in yyyy-MM-dd format" });

        var result = await _nutritionService.AddMealLogAsync(parsedDate, dto, CurrentUserId);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return StatusCode(201, result.Data);
    }

    /// <summary>Remove a meal log entry</summary>
    [HttpDelete("meals/{mealLogId:guid}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> RemoveMeal(Guid mealLogId)
    {
        var result = await _nutritionService.RemoveMealLogAsync(mealLogId, CurrentUserId);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return NoContent();
    }

    /// <summary>Get nutrition history for the last N days</summary>
    [HttpGet("history")]
    [ProducesResponseType(typeof(IEnumerable<DailyNutritionLogDto>), 200)]
    public async Task<IActionResult> GetHistory([FromQuery] int days = 30)
    {
        var result = await _nutritionService.GetHistoryAsync(CurrentUserId, days);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return Ok(result.Data);
    }
}
