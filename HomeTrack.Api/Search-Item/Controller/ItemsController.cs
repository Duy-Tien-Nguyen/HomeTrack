using Microsoft.AspNetCore.Mvc;
using HomeTrack.Api.Request;
using HomeTrack.Application.Interface;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace HomeTrack.Api.Controllers
{
    [ApiController]
    [Route("api/items")]
    public class ItemsController : ControllerBase
    {
        private readonly IItemService _itemService;
        private readonly IWebHostEnvironment _hostEnvironment;
        private string GetMimeTypeForFileExtension(string filePath)
        {
            const string defaultContentType = "application/octet-stream";
            var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
            if (!provider.TryGetContentType(filePath, out var contentType))
            {
                contentType = defaultContentType;
            }
            return contentType;
        }


        public ItemsController(IItemService itemService, IWebHostEnvironment hostEnvironment)
        {
            _itemService = itemService;
            _hostEnvironment = hostEnvironment;
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

        [HttpGet("{id}/image")]
        [Authorize]
        public async Task<IActionResult> GetItemImage(int id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác định người dùng từ token." });
            }

            var item = await _itemService.GetItemByIdAsync(id, userId);

            if (!item.IsSuccess || item.Data == null)
            {
                return NotFound(new { message = "Đồ vật này không có hình ảnh." });
            }

            var imagePath = Path.Combine(_hostEnvironment.WebRootPath) ?? Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            var filePath = Path.Combine(imagePath, item.Data.ImageUrl.TrimStart('/'));

            Console.WriteLine($"File path: {filePath}");

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound(new { message = "Không tìm thấy hình ảnh." });
            }

            var mimeType = GetMimeTypeForFileExtension(filePath);
            return PhysicalFile(filePath, mimeType);
        }
    }
}