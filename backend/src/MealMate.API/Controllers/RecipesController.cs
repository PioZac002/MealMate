using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MealMate.Application.Common;
using MealMate.Application.DTOs.Recipe;
using MealMate.Application.Interfaces;

namespace MealMate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RecipesController : ControllerBase
{
    private readonly IRecipeService _recipeService;

    public RecipesController(IRecipeService recipeService)
    {
        _recipeService = recipeService;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)
        ?? Guid.Empty.ToString());

    /// <summary>Get all recipes with optional filtering and pagination</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<RecipeDto>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] RecipeFilterDto filter)
    {
        var userId = User.Identity?.IsAuthenticated == true
            ? Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)
            : Guid.Empty;
        var result = await _recipeService.GetAllAsync(filter, userId);
        return Ok(result.Data);
    }

    /// <summary>Get recipe details by ID</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(RecipeDetailDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var userId = User.Identity?.IsAuthenticated == true
            ? Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!)
            : Guid.Empty;
        var result = await _recipeService.GetByIdAsync(id, userId);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return Ok(result.Data);
    }

    /// <summary>Create a new recipe</summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(RecipeDetailDto), 201)]
    public async Task<IActionResult> Create([FromBody] CreateRecipeDto dto)
    {
        var result = await _recipeService.CreateAsync(dto, CurrentUserId);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    /// <summary>Update a recipe (owner only)</summary>
    [HttpPut("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(RecipeDetailDto), 200)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRecipeDto dto)
    {
        var result = await _recipeService.UpdateAsync(id, dto, CurrentUserId);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return Ok(result.Data);
    }

    /// <summary>Delete a recipe (owner only)</summary>
    [HttpDelete("{id:guid}")]
    [Authorize]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _recipeService.DeleteAsync(id, CurrentUserId);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return NoContent();
    }
}
