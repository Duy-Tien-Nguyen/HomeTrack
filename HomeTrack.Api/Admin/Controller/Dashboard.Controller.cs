using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using HomeTrack.Application.Interface;
using HomeTrack.Domain.Enum;

namespace HomeTrack.Api.Controller
{
  [Authorize(Roles = "Admin")]
  [Route("api/[controller]")]
  [ApiController]

  public class DashboardController : ControllerBase
  {
    private readonly IDashboardRepository _dashboardRepository;

    public DashboardController(IDashboardRepository dashboardRepository)
    {
      _dashboardRepository = dashboardRepository;
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
  }
}