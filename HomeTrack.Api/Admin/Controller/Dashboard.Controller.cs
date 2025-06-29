using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HomeTrack.Application.Interface;
using HomeTrack.Api.Request;

namespace HomeTrack.Api.Controller
{
  [Authorize(Roles = "Admin")]
  [Route("api/[controller]")]
  [ApiController]

  public class DashboardController : ControllerBase
  {
    private readonly IDashboardRepository _dashboardRepository;
    private readonly IAdminService _adminService;

    public DashboardController(IDashboardRepository dashboardRepository, IAdminService adminService)
    {
      _dashboardRepository = dashboardRepository;
      _adminService = adminService;
    }

    // GET: api/dashboard/user-registrations-by-month?year=2024
    [HttpGet("user-registrations-by-month")]
    public async Task<IActionResult> GetUserRegistrationsByMonth([FromQuery] int year)
    {
      if (year <= 0) year = DateTime.UtcNow.Year;
      var data = await _dashboardRepository.GetUserRegistrationsByMonthAsync(year);
      return Ok(data);
    }

    [HttpGet("new-items-by-month")]
    public async Task<IActionResult> GetNewItemsByMonth([FromQuery] int year)
    {
      if (year <= 0) year = DateTime.UtcNow.Year;
      var data = await _dashboardRepository.GetNewItemsByMonthAsync(year);
      return Ok(data);
    }

    // GET: api/dashboard/summary
    [HttpGet("summary")]
    public async Task<IActionResult> GetDashboardSummary()
    {
      var summary = await _dashboardRepository.GetDashboardSummaryAsync();
      return Ok(summary);
    }

    [HttpGet("statistics/users-by-role")]
    [ProducesResponseType(typeof(IEnumerable<UserCountByRoleDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetUserCountByRole()
    {
      try
      {
        // Gọi qua service nếu bạn đã triển khai ở Bước 3
        var result = await _adminService.GetUserCountByRoleAsync();
        if (!result.IsSuccess)
        {
          // Trả về lỗi nếu service báo lỗi
          return StatusCode(StatusCodes.Status500InternalServerError, new { message = result.ErrorMessage });
        }

        return Ok(result.Data); // Trả về kết quả từ service
      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã có lỗi xảy ra ở phía máy chủ." });
      }
    }
  }
}