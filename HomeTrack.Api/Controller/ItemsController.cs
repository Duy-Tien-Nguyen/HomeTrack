using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using HomeTrack.Api.Models.DTOs;
using HomeTrack.Api.Services; 
using System.Threading.Tasks; 
using System.Security.Claims; 
using Microsoft.AspNetCore.Authorization; 
using System.Collections.Generic;

namespace HomeTrack.Api.Controller
{
[ApiController]
[Route("api/items")]
public class ItemsController : ControllerBase
{
    private readonly IItemService _itemService; 

    public ItemsController(IItemService itemService)
    {
        _itemService = itemService;
    }

    [Authorize] 
    [HttpPost("create")]
    public async Task<IActionResult> CreateItem([FromForm] CreateItemDto itemDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        {
            return Unauthorized(new { message = "Không thể xác định người dùng từ token." });
        }

        var result = await _itemService.CreateNewItem(itemDto, userId);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        if (result.Data == null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã tạo Item thành công nhưng dữ liệu trả về bị thiếu." });
        }

        return CreatedAtAction(nameof(GetItemById), new { id = result.Data.Id }, result.Data);
    }

    [Authorize] 
    [HttpGet("getById/{id}")]
    public async Task<IActionResult> GetItemById(int id) 
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        {
            return Unauthorized(new { message = "Không thể xác định người dùng từ token." });
        }

        var result = await _itemService.GetItemByIdAsync(id, userId);

        if (!result.IsSuccess)
        {
            return NotFound(new { message = result.ErrorMessage });
        }

        if (result.Data == null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã lấy Item thành công nhưng dữ liệu trả về bị thiếu." });
        }

        return Ok(result.Data);
    }

    [Authorize]
    [HttpPut("update/{id}")]
    public async Task<IActionResult> UpdateItem(int id, [FromForm] ItemUpdateRequestDto itemDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        {
            return Unauthorized(new { message = "Không thể xác định người dùng từ token." });
        }

        var result = await _itemService.UpdateItemAsync(id, itemDto, userId);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        if (result.Data == null)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã cập nhật Item thành công nhưng dữ liệu trả về bị thiếu." });
        }

        return Ok(result.Data); 
    }

    [Authorize]
    [HttpDelete("delete/{id}")]
    public async Task<IActionResult> DeleteItem(int id)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        {
            return Unauthorized(new { message = "Không thể xác định người dùng từ token." });
        }

        var result = await _itemService.DeleteItemAsync(id, userId);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        return NoContent(); 
    }

    [Authorize]
    [HttpGet("by-location/{locationId}")]
    public async Task<IActionResult> GetItemsByLocation(int locationId)
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
        {
            return Unauthorized(new { message = "Không thể xác định người dùng từ token." });
        }

        var result = await _itemService.GetItemsByLocationAsync(locationId, userId);

        if (!result.IsSuccess)
        {
            return BadRequest(new { message = result.ErrorMessage });
        }

        if (result.Data == null)
        {
            return Ok(new List<ItemViewModel>());
        }

        return Ok(result.Data);
    }
}
}