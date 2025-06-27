using Microsoft.AspNetCore.Mvc;
using HomeTrack.Application.Interface;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using HomeTrack.Api.Request;


namespace HomeTrack.Api.Controllers
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

    [Authorize]
    [HttpPost("advanced")]
    public async Task<IActionResult> AdvancedSearchItems([FromBody] AdvancedSearchRequestDto req)
    {
      // Extract userId from claims
      var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
      if (userIdClaim == null || !int.TryParse(userIdClaim.Value, out int userId))
      {
        return Unauthorized(new { message = "Không thể xác thực người dùng." });
      }

      if (req == null)
      {
        return BadRequest(new { message = "Yêu cầu tìm kiếm nâng cao không hợp lệ." });
      }

      var result = await _searchService.AdvancedSearchItemsAsync(req, userId);

      if (!result.IsSuccess)
      {
        return BadRequest(new { message = result.ErrorMessage });
      }

      return Ok(result.Data);
    }
  }
}
