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
        private readonly IGoogleAIStudioModerationService _aIStudioModerationService;
        private readonly ILogger<ItemsController> _logger;
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


        public ItemsController(IItemService itemService, IWebHostEnvironment hostEnvironment,
            IGoogleAIStudioModerationService aIStudioModerationService,
            ILogger<ItemsController> logger)
        {
            _itemService = itemService;
            _hostEnvironment = hostEnvironment;
            _logger = logger;
            _aIStudioModerationService = aIStudioModerationService;
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

            if (!item.IsSuccess || item.Data == null || string.IsNullOrEmpty(item.Data.ImageUrl))
            {
                return NotFound(new { message = "Đồ vật này không có hình ảnh hoặc không tìm thấy." });
            }

            var itemData = item.Data;

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

        [Authorize]
        [HttpPost("suggest-tags-for-image")]
        // [Consumes("multipart/from-data")]
        // [ProducesResponseType(typeof(AISuggestedTagResult), StatusCodes.Status200OK)]
        // [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status400BadRequest)]
        // [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> SuggestTagsForUploadedImage([FromForm] SuggestTagsRequestDto request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }
            if (request.ImageFile == null || request.ImageFile.Length == 0)
            {
                return BadRequest(new ProblemDetails { Title = "File ảnh là bát buộc", Status = StatusCodes.Status400BadRequest });
            }
            if (request.ImageFile.Length > 5 * 1024 * 1024)
            {
                return BadRequest(new ProblemDetails { Title = "Kích thước file ảnh không được vượt quá 5MB.", Status = StatusCodes.Status400BadRequest });
            }
            var allowedMimeTypes = new[] { "image/jpeg", "image/png" };
            if (!allowedMimeTypes.Contains(request.ImageFile.ContentType.ToLowerInvariant()))
            {
                return BadRequest(new ProblemDetails { Title = "Định dạng file ảnh không được hỗ trợ. Chỉ chấp nhận JPEG, PNG", Status = StatusCodes.Status400BadRequest });
            }

            try
            {
                // Mở stream từ IFormFile
                await using var imageStream = request.ImageFile.OpenReadStream();

                var result = await _aIStudioModerationService.SuggestTagsForImageAsync(imageStream, request.ImageFile.ContentType, request.ItemContextText);

                if (result.IsSuccess)
                {
                    return Ok(result); // Trả về AISuggestedTagResult chứa danh sách tag
                }
                else
                {
                    // Log lỗi chi tiết từ AI Service nếu có
                    _logger.LogWarning("AI Tag Suggestion failed: {ErrorMessage}. Raw Response: {RawResponse}", result.ErrorMessage, result.RawApiResponse);
                    // Trả về lỗi cho client một cách thân thiện hơn
                    return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                    {
                        Title = "Không thể gợi ý tag vào lúc này.",
                        Detail = result.ErrorMessage ?? "Đã có lỗi xảy ra từ dịch vụ AI.",
                        Status = StatusCodes.Status500InternalServerError
                    });
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi ngoại lệ khi thực hiện gợi ý tag cho ảnh.");
                return StatusCode(StatusCodes.Status500InternalServerError, new ProblemDetails
                {
                    Title = "Đã có lỗi hệ thống xảy ra.",
                    Detail = "Không thể xử lý yêu cầu gợi ý tag do lỗi hệ thống.",
                    Status = StatusCodes.Status500InternalServerError
                });
            }
        }
    }
}