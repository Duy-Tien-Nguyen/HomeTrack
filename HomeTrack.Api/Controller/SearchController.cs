using Microsoft.AspNetCore.Mvc;
using HomeTrack.Api.Models.DTOs;
using HomeTrack.Api.Services;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace HomeTrack.Api.Controller
{
    [ApiController]
    [Route("api/search")]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;

        public SearchController(ISearchService searchService)
        {
            _searchService = searchService;
        }

        [Authorize]
        [HttpGet("items")]
        public async Task<IActionResult> BasicSearchItems([FromQuery] string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
            {
                return BadRequest(new { message = "Từ khóa tìm kiếm không được để trống." });
            }

            var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
            {
                return Unauthorized(new { message = "Không thể xác định người dùng từ token." });
            }

            var result = await _searchService.BasicSearchItemsAsync(keyword, userId);

            if (!result.IsSuccess)
            {
                return BadRequest(new { message = result.ErrorMessage });
            }

            return Ok(result.Data);
        }
    }
}
