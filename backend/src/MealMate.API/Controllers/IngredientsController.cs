using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MealMate.Application.Common;
using MealMate.Application.DTOs.Ingredient;
using MealMate.Application.Interfaces;

namespace MealMate.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class IngredientsController : ControllerBase
{
    private readonly IIngredientService _ingredientService;

    public IngredientsController(IIngredientService ingredientService)
    {
        _ingredientService = ingredientService;
    }

    /// <summary>Get all ingredients with optional filtering and pagination</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<IngredientDto>), 200)]
    public async Task<IActionResult> GetAll([FromQuery] IngredientFilterDto filter)
    {
        var result = await _ingredientService.GetAllAsync(filter);
        return Ok(result.Data);
    }

    /// <summary>Get ingredient by ID</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(IngredientDto), 200)]
    [ProducesResponseType(404)]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _ingredientService.GetByIdAsync(id);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return Ok(result.Data);
    }

    /// <summary>Create a new ingredient</summary>
    [HttpPost]
    [Authorize]
    [ProducesResponseType(typeof(IngredientDto), 201)]
    public async Task<IActionResult> Create([FromBody] CreateIngredientDto dto)
    {
        var result = await _ingredientService.CreateAsync(dto);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return CreatedAtAction(nameof(GetById), new { id = result.Data!.Id }, result.Data);
    }

    /// <summary>Update an ingredient</summary>
    [HttpPut("{id:guid}")]
    [Authorize]
    [ProducesResponseType(typeof(IngredientDto), 200)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateIngredientDto dto)
    {
        var result = await _ingredientService.UpdateAsync(id, dto);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return Ok(result.Data);
    }

    /// <summary>Delete an ingredient</summary>
    [HttpDelete("{id:guid}")]
    [Authorize]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _ingredientService.DeleteAsync(id);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return NoContent();
    }
}
