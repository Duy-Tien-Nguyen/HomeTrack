using Microsoft.AspNetCore.Mvc;
using HomeTrack.Application.Interface;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace HomeTrack.Api.Controllers
{
    [ApiController]
    [Route("api/statistics")]
    [Authorize] // Yêu cầu xác thực cho tất cả các endpoint thống kê
    public class StatisticsController : ControllerBase
    {
        private readonly IStatisticsService _statisticsService;

        public StatisticsController(IStatisticsService statisticsService)
        {
            _statisticsService = statisticsService;
        }

        private int GetUserIdFromClaims()
        {
            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                throw new UnauthorizedAccessException("Không thể xác định người dùng từ token.");
            }
            return userId;
        }

        [HttpGet("usage")]
        public async Task<IActionResult> GetItemUsageStats([FromQuery] string timeframe)
        {
            try
            {
                var userId = GetUserIdFromClaims();
                var result = await _statisticsService.GetItemUsageStatsAsync(timeframe, userId);

                if (!result.IsSuccess)
                {
                    return BadRequest(new { message = result.ErrorMessage });
                }
                return Ok(result.Data);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Đã xảy ra lỗi nội bộ: {ex.Message}" });
            }
        }

        [HttpGet("top-moved")]
        public async Task<IActionResult> GetTopMovedItems([FromQuery] string timeframe)
        {
            try
            {
                var userId = GetUserIdFromClaims();
                var result = await _statisticsService.GetTopMovedItemsAsync(timeframe, userId);

                if (!result.IsSuccess)
                {
                    return BadRequest(new { message = result.ErrorMessage });
                }
                return Ok(result.Data);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Đã xảy ra lỗi nội bộ: {ex.Message}" });
            }
        }

        [HttpGet("least-used")]
        public async Task<IActionResult> GetLeastUsedItems([FromQuery] string timeframe)
        {
            try
            {
                var userId = GetUserIdFromClaims();
                var result = await _statisticsService.GetLeastUsedItemsAsync(timeframe, userId);

                if (!result.IsSuccess)
                {
                    return BadRequest(new { message = result.ErrorMessage });
                }
                return Ok(result.Data);
            }
            catch (UnauthorizedAccessException ex)
            {
                return Unauthorized(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Đã xảy ra lỗi nội bộ: {ex.Message}" });
            }
        }
    }
}
