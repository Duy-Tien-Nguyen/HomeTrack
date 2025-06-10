using System.Security.Claims;
using HomeTrack.Api.Request;
using HomeTrack.Application.Interface;
using HomeTrack.Domain.Enum;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HomeTrack.Api.Controllers
{
  [ApiController]
  [Route("api/admin")]
  [Authorize(Roles = "Admin")]
  public class AdminController : ControllerBase
  {
    private readonly IAdminService _adminService;
    private readonly ISystemSettingRepository _systemSettingRepository;

    public AdminController(IAdminService adminService, ISystemSettingRepository systemSettingRepository)
    {
      _adminService = adminService;
      _systemSettingRepository = systemSettingRepository;
    }

    [HttpGet("user/all")]
    public async Task<IActionResult> GetAllUsers()
    {
      try
      {
        var users = await _adminService.GetAllUser();
        if (users == null || !users.Any())
          return NotFound("Không có người dùng nào được tìm thấy.");

        return Ok(users);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Lỗi khi lấy danh sách người dùng: {ex.Message}");
      }
    }

    [HttpGet("user/{userId}")]
    public async Task<IActionResult> ViewUserDetail(int userId)
    {
      try
      {
        var user = await _adminService.ViewUserDetail(userId);
        if (user == null)
          return NotFound($"Không tìm thấy người dùng với ID {userId}.");

        return Ok(user);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Lỗi khi xem chi tiết người dùng: {ex.Message}");
      }
    }

    [HttpPost("user/upgrade/{userId}")]
    public async Task<IActionResult> UpgradeUser(int userId)
    {
      try
      {
        var result = await _adminService.UpgradeDowngrade(userId, PackageType.Premium);
        if (!result)
          return BadRequest($"Thất bại khi nâng cấp gói cho người dùng ID {userId}.");

        return Ok($"Người dùng với ID {userId} đã được nâng cấp lên gói Premium.");
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Lỗi khi nâng cấp gói: {ex.Message}");
      }
    }

    [HttpPost("user/downgrade/{userId}")]
    public async Task<IActionResult> DowngradeUser(int userId)
    {
      try
      {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        if (userIdClaim != null && int.TryParse(userIdClaim, out var currentUserId) && currentUserId == userId)
          return BadRequest("Admin không thể tự hạ cấp chính mình.");

        var result = await _adminService.UpgradeDowngrade(userId, PackageType.Basic);
        if (!result)
          return BadRequest($"Thất bại khi hạ cấp gói cho người dùng ID {userId}.");

        return Ok($"Người dùng với ID {userId} đã được hạ cấp xuống gói Basic.");
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Lỗi khi hạ cấp gói: {ex.Message}");
      }
    }

    [HttpPost("user/ban/{userId}")]
    public async Task<IActionResult> BanUser(int userId)
    {
      try
      {
        var result = await _adminService.BanUnlock(userId, UserStatus.Banned);
        if (!result)
          return BadRequest($"Thất bại khi ban người dùng ID {userId}.");

        return Ok($"Người dùng với ID {userId} đã bị ban.");
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Lỗi khi ban người dùng: {ex.Message}");
      }
    }

    [HttpPost("user/unlock/{userId}")]
    public async Task<IActionResult> UnlockUser(int userId)
    {
      try
      {
        var result = await _adminService.BanUnlock(userId, UserStatus.Active);
        if (!result)
          return BadRequest($"Thất bại khi mở khóa người dùng ID {userId}.");

        return Ok($"Người dùng với ID {userId} đã được mở khóa.");
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Lỗi khi mở khóa người dùng: {ex.Message}");
      }
    }

    [HttpGet("package/all")]
    public async Task<IActionResult> GetAllPackages()
    {
      try
      {
        var packages = await _systemSettingRepository.GetAllAsync();
        if (packages == null || !packages.Any())
          return NotFound("Không có gói nào được tìm thấy.");

        return Ok(packages);
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Lỗi khi lấy danh sách gói: {ex.Message}");
      }
    }

    [HttpPost("package/change-basic-limit/{newLimit}")]
    public async Task<IActionResult> ChangeBasicPackageLimit(int newLimit)
    {
      try
      {
        var result = await _adminService.ChangeItemLimit(PackageType.Basic, newLimit);
        if (!result)
          return BadRequest("Thất bại khi thay đổi giới hạn gói Basic.");

        return Ok($"Giới hạn gói Basic đã được cập nhật thành {newLimit}.");
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Lỗi khi thay đổi giới hạn: {ex.Message}");
      }
    }

    [HttpPost("package/change-premium-limit/{newLimit}")]
    public async Task<IActionResult> ChangePremiumPackageLimit(int newLimit)
    {
      try
      {
        var result = await _adminService.ChangeItemLimit(PackageType.Premium, newLimit);
        if (!result)
          return BadRequest("Thất bại khi thay đổi giới hạn gói Premium.");

        return Ok($"Giới hạn gói Premium đã được cập nhật thành {newLimit}.");
      }
      catch (Exception ex)
      {
        return StatusCode(500, $"Lỗi khi thay đổi giới hạn: {ex.Message}");
      }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("logs")]
    public async Task<IActionResult> GetLogs([FromQuery] int? userId, [FromQuery] string? actionType, [FromQuery] DateTime? startTime, [FromQuery] DateTime? endTime)
    {
      try
      {
        var result = await _adminService.GetSystemLogsAsync(userId, actionType, startTime, endTime);

        if (!result.IsSuccess)
        {
          return StatusCode(StatusCodes.Status500InternalServerError, new { message = result.ErrorMessage });
        }

        return Ok(result.Data);
      }
      catch (Exception ex) // Bắt các lỗi
      {
        return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Đã xảy ra lỗi không mong muốn: {ex.Message}" });
      }
    }

    [HttpGet("statistics/users-per-month")]
    public async Task<IActionResult> GetUsersPerMonth()
    {
      try
      {
        var result = await _adminService.GetUserRegistrationsPerMonthAsync();
        if (!result.IsSuccess)
        {
          return StatusCode(StatusCodes.Status500InternalServerError, new { message = result.ErrorMessage });
        }
        return Ok(result.Data);
      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Đã xảy ra lỗi không mong muốn: {ex.Message}" });
      }
    }

    [Authorize(Roles = "Admin")]
    [HttpGet("statistics/items-per-month")]
    public async Task<IActionResult> GetItemsPerMonth()
    {
      try
      {
        var result = await _adminService.GetItemCreationsPerMonthAsync();
        if (!result.IsSuccess)
        {
          return StatusCode(StatusCodes.Status500InternalServerError, new { message = result.ErrorMessage });
        }
        return Ok(result.Data);
      }
      catch (Exception ex)
      {
        return StatusCode(StatusCodes.Status500InternalServerError, new { message = $"Đã xảy ra lỗi không mong muốn: {ex.Message}" });
      }
    }
  }
}
