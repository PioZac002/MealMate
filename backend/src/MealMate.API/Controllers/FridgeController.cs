using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MealMate.Application.DTOs.Fridge;
using MealMate.Application.Interfaces;

namespace MealMate.API.Controllers;

[ApiController]
[Route("api/households/{householdId:guid}/fridge")]
[Authorize]
public class FridgeController : ControllerBase
{
    private readonly IFridgeService _fridgeService;

    public FridgeController(IFridgeService fridgeService)
    {
        _fridgeService = fridgeService;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Get all fridge items for a household</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<FridgeItemDto>), 200)]
    public async Task<IActionResult> GetAll(Guid householdId)
    {
        var result = await _fridgeService.GetByHouseholdAsync(householdId, CurrentUserId);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return Ok(result.Data);
    }

    /// <summary>Add an item to the fridge</summary>
    [HttpPost]
    [ProducesResponseType(typeof(FridgeItemDto), 201)]
    public async Task<IActionResult> Add(Guid householdId, [FromBody] CreateFridgeItemDto dto)
    {
        var result = await _fridgeService.AddItemAsync(householdId, dto, CurrentUserId);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return StatusCode(201, result.Data);
    }

    /// <summary>Update a fridge item</summary>
    [HttpPut("{itemId:guid}")]
    [ProducesResponseType(typeof(FridgeItemDto), 200)]
    public async Task<IActionResult> Update(Guid householdId, Guid itemId, [FromBody] UpdateFridgeItemDto dto)
    {
        var result = await _fridgeService.UpdateItemAsync(itemId, dto, CurrentUserId);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return Ok(result.Data);
    }

    /// <summary>Delete a fridge item</summary>
    [HttpDelete("{itemId:guid}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(Guid householdId, Guid itemId)
    {
        var result = await _fridgeService.DeleteItemAsync(itemId, CurrentUserId);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return NoContent();
    }
}
