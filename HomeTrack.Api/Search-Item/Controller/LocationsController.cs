using Microsoft.AspNetCore.Mvc;
using HomeTrack.Api.Request;
using HomeTrack.Application.Interface; 
using System.Security.Claims;  
using Microsoft.AspNetCore.Authorization; 

namespace HomeTrack.Api.Controller
{
    [ApiController]
    [Route("api/locations")]
    public class LocationsController : ControllerBase
    {
        private readonly ILocationService _locationService; 

        public LocationsController(ILocationService locationService)
        {
            _locationService = locationService;
        }

        [Authorize] 
        [HttpPost("create")]
        public async Task<IActionResult> CreateLocation([FromBody] LocationCreateRequestDto locationDto)
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

            var result = await _locationService.CreateLocationAsync(locationDto, userId);

            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.ErrorMessage });
            }

            if (result.Data == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã tạo Location thành công nhưng dữ liệu trả về bị thiếu." });
            }

            return CreatedAtAction(nameof(GetLocationById), new { id = result.Data.Id }, result.Data);
        }

        [Authorize]
        [HttpGet("getAll")]
        public async Task<IActionResult> GetLocations()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác định người dùng từ token." });
            }

            var result = await _locationService.GetLocationsAsync(userId);

            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.ErrorMessage });
            }

            if (result.Data == null)
            {
                return Ok(new List<LocationResponseDto>());
            }

            return Ok(result.Data);
        }

        [Authorize]
        [HttpGet("getById/{id}")]
        public async Task<IActionResult> GetLocationById(int id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác định người dùng từ token." });
            }

            var result = await _locationService.GetLocationByIdAsync(id, userId);

            if (!result.IsSuccess)
            {
                return NotFound(new { message = result.ErrorMessage });
            }

            if (result.Data == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã lấy Location thành công nhưng dữ liệu trả về bị thiếu." });
            }

            return Ok(result.Data);
        }

        [Authorize]
        [HttpPut("update/{id}")]
        public async Task<IActionResult> UpdateLocation(int id, [FromBody] LocationUpdateRequestDto locationDto)
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

            var result = await _locationService.UpdateLocationAsync(id, locationDto, userId);

            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.ErrorMessage });
            }

            if (result.Data == null)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã cập nhật Location thành công nhưng dữ liệu trả về bị thiếu." });
            }

            return Ok(result.Data);
        }

        [Authorize]
        [HttpDelete("delete/{id}")]
        public async Task<IActionResult> DeleteLocation(int id)
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác định người dùng từ token." });
            }

            var result = await _locationService.DeleteLocationAsync(id, userId);

            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.ErrorMessage });
            }

            return NoContent();
        }
    }
}
