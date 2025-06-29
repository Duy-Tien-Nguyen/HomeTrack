using Microsoft.AspNetCore.Mvc;
using HomeTrack.Application.Interface;
using HomeTrack.Api.Request;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace HomeTrack.Api.Controllers
{
  [ApiController]
  [Route("api/auth")]
  public class AuthController : ControllerBase
  {
    private readonly IAuthService _authService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthService authService, ILogger<AuthController> logger)
    {
      _authService = authService;
      _logger = logger;
    }

    [HttpPost("login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginRequest req)
    {
      var result = await _authService.LoginAsync(req);
      if (result == null)
      {
        return Unauthorized(new { message = "Đăng nhập thất bại. Vui lòng kiểm tra lại thông tin." });
      }
      return Ok(result);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
      var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

      if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
      {
        _logger.LogWarning("Logout failed: Invalid or missing user ID in token.");
        return Unauthorized(new { message = "Không thể xác định người dùng từ token." });
      }

      try
      {
        bool success = await _authService.LogoutAsync(userId);
        if (success)
        {
          _logger.LogInformation("User {UserId} logged out successfully.", userId);
          return Ok(new { message = "Đăng xuất thành công." });
        }
        else
        {
          _logger.LogWarning("Logout for user {UserId} did not complete successfully.", userId);
          return Ok(new { message = "Yêu cầu đăng xuất đã được xử lý." });
        }
      }
      catch (Exception ex)
      {
        _logger.LogError(ex, "Error during logout for UserId {UserId}.", userId);
        return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Lỗi khi đăng xuất." });
      }
    }

    [Authorize]
    [HttpGet("access_token")]
    public async Task<IActionResult> GetAccessToken()
    {
      var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
      var email = User.FindFirstValue(ClaimTypes.Email);
      var role = User.FindFirstValue(ClaimTypes.Role);

      var token = await _authService.GetAccessToken(userId, email, role);
      if (token == null)
      {
        return Unauthorized(new { message = "Không thể tạo access token." });
      }

      return Ok(token);
    }

    [Authorize]
    [HttpPatch("reset_password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest req)
    {
      var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);

      if (!int.TryParse(userIdStr, out int userId))
      {
        return Unauthorized(new { message = "Không thể xác định người dùng từ token." });
      }

      var success = await _authService.ResetPassword(userId, req.oldPassword, req.newPassword);
      if (!success)
      {
        return BadRequest(new { message = "Đổi mật khẩu thất bại." });
      }

      return Ok(new { message = "Đổi mật khẩu thành công." });
    }

    [HttpPatch("forgot_password")]
    public async Task<IActionResult> ForgetPassword([FromBody] ForgetPasswordRequest req)
    {
      if (req.newPassword != req.repeatPassword)
      {
        return BadRequest(new { message = "Mật khẩu lặp lại không khớp." });
      }

      var result = await _authService.ForgotPassword(req.token, req.email, req.newPassword);
      if (!result)
      {
        return BadRequest(new { message = "Đổi mật khẩu thất bại." });
      }

      return Ok(new { message = "Đổi mật khẩu thành công." });
    }

    [Authorize]
    [HttpGet("myprofile")]
    public async Task<IActionResult> GetMyProfile()
    {
      var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
      if (!int.TryParse(userIdStr, out int userId))
      {
        return Unauthorized(new { message = "Không thể xác định người dùng từ token." });
      }

      var user = await _authService.GetProfile(userId);
      if (user == null)
      {
        return NotFound(new { message = "Không tìm thấy người dùng." });
      }

      return Ok(user);
    }
  }
}
