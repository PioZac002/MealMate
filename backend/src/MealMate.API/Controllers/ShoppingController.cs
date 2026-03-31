using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MealMate.Application.DTOs.Shopping;
using MealMate.Application.Interfaces;

namespace MealMate.API.Controllers;

[ApiController]
[Route("api/households/{householdId:guid}/shopping-lists")]
[Authorize]
public class ShoppingController : ControllerBase
{
    private readonly IShoppingService _shoppingService;

    public ShoppingController(IShoppingService shoppingService)
    {
        _shoppingService = shoppingService;
    }

    private Guid CurrentUserId => Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

    /// <summary>Get all shopping lists for a household</summary>
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<ShoppingListDto>), 200)]
    public async Task<IActionResult> GetAll(Guid householdId)
    {
        var result = await _shoppingService.GetByHouseholdAsync(householdId, CurrentUserId);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return Ok(result.Data);
    }

    /// <summary>Get a shopping list with items</summary>
    [HttpGet("{listId:guid}")]
    [ProducesResponseType(typeof(ShoppingListDetailDto), 200)]
    public async Task<IActionResult> GetById(Guid householdId, Guid listId)
    {
        var result = await _shoppingService.GetByIdAsync(listId, CurrentUserId);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return Ok(result.Data);
    }

    /// <summary>Create a new shopping list</summary>
    [HttpPost]
    [ProducesResponseType(typeof(ShoppingListDetailDto), 201)]
    public async Task<IActionResult> Create(Guid householdId, [FromBody] CreateShoppingListDto dto)
    {
        var result = await _shoppingService.CreateAsync(householdId, dto, CurrentUserId);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return StatusCode(201, result.Data);
    }

    /// <summary>Add an item to a shopping list</summary>
    [HttpPost("{listId:guid}/items")]
    [ProducesResponseType(typeof(ShoppingListItemDto), 201)]
    public async Task<IActionResult> AddItem(Guid householdId, Guid listId, [FromBody] AddShoppingListItemDto dto)
    {
        var result = await _shoppingService.AddItemAsync(listId, dto, CurrentUserId);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return StatusCode(201, result.Data);
    }

    /// <summary>Toggle an item bought/unbought</summary>
    [HttpPatch("{listId:guid}/items/{itemId:guid}/toggle")]
    [ProducesResponseType(typeof(ShoppingListItemDto), 200)]
    public async Task<IActionResult> ToggleItem(Guid householdId, Guid listId, Guid itemId)
    {
        var result = await _shoppingService.ToggleItemAsync(listId, itemId, CurrentUserId);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return Ok(result.Data);
    }

    /// <summary>Remove an item from a shopping list</summary>
    [HttpDelete("{listId:guid}/items/{itemId:guid}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> RemoveItem(Guid householdId, Guid listId, Guid itemId)
    {
        var result = await _shoppingService.RemoveItemAsync(listId, itemId, CurrentUserId);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return NoContent();
    }

    /// <summary>Mark the shopping list as completed</summary>
    [HttpPost("{listId:guid}/complete")]
    [ProducesResponseType(typeof(ShoppingListDetailDto), 200)]
    public async Task<IActionResult> Complete(Guid householdId, Guid listId)
    {
        var result = await _shoppingService.CompleteListAsync(listId, CurrentUserId);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return Ok(result.Data);
    }

    /// <summary>Delete a shopping list</summary>
    [HttpDelete("{listId:guid}")]
    [ProducesResponseType(204)]
    public async Task<IActionResult> Delete(Guid householdId, Guid listId)
    {
        var result = await _shoppingService.DeleteAsync(listId, CurrentUserId);
        if (!result.Success)
            return StatusCode(result.StatusCode, new { error = result.Error });
        return NoContent();
    }
}
