using HomeTrack.Application.Interface;
using Microsoft.AspNetCore.Authorization;
using HomeTrack.Api.Request;
using Microsoft.AspNetCore.Mvc;

namespace HomeTrack.Api.Controllers
{
  [ApiController]
  [Authorize(Roles = "Admin")]
  [Route("api/[controller]")]
  public class AdminLogController : ControllerBase
  {
    private readonly IStatsReportRepository _statsReportRepository;

    public AdminLogController(IStatsReportRepository statsReportRepository)
    {
      _statsReportRepository = statsReportRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetStatsReports([FromQuery] LogQueryParameters queryParameters)
    {
      var pagedResult = await _statsReportRepository.GetStatsReportsAsync(queryParameters);
      return Ok(pagedResult);
    }
  }
}