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
    public async Task<LoginResponseDto> LoginAsync([FromBody] LoginRequest req)
    {
      return await _authService.LoginAsync(req);
    }

    [Authorize]
    [HttpPost("logout")]
    public async Task<IActionResult> Logout()
    {
      var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

      if (string.IsNullOrEmpty(userIdString))
      {
        _logger.LogWarning("Logout attempt failed: UserId claim (NameIdentifier) not found in token.");

        return Unauthorized(new { message = "Không thể xác định người dùng từ token." });
      }

      // Chuyển đổi userIdString sang int
      if (int.TryParse(userIdString, out int userId))
      {
        try
        {
          bool logoutSuccess = await _authService.LogoutAsync(userId);
          if (logoutSuccess)
          {
            _logger.LogInformation("User {UserId} logged out successfully.", userId);
            return Ok(new { message = "Đăng xuất thành công." });
          }
          else
          {

            _logger.LogWarning("Logout process for UserId {UserId} did not complete successfully (as reported by AuthService).", userId);
            return Ok(new { message = "Yêu cầu đăng xuất đã được xử lý." });
          }
        }
        catch (Exception ex)
        {
          _logger.LogError(ex, "An error occurred during logout for UserId {UserIdString}.", userIdString);
          return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Đã có lỗi xảy ra trong quá trình đăng xuất." });
        }
      }
      else
      {
        _logger.LogError("Logout attempt failed: UserId claim (NameIdentifier) '{UserIdString}' is not a valid integer.", userIdString);
        return BadRequest(new { message = "Định dạng ID người dùng không hợp lệ trong token." });
      }
    }

    [Authorize]
    [HttpGet("access_token")]
    public Task<AccessTokenString> GetAccessToken()
    {
      var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var email = User.FindFirst(ClaimTypes.Email)?.Value;
      var role = User.FindFirst(ClaimTypes.Role)?.Value;

      return _authService.GetAccessToken(userId, email, role);
    }

    [Authorize]
    [HttpPatch("reset_password")]
    public async Task<bool> ResetPassword([FromBody] ResetPasswordRequest req)
    {
      var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

      int.TryParse(userId, out int userIdInt);

      bool result = await _authService.ResetPassword(userIdInt, req.newPassword);
      return result;
    }

    [HttpPatch("forgot_password")]
    public async Task<bool> ForgetPassword([FromBody] ForgetPasswordRequest req)
    {
      if (req.newPassword != req.repeatPassword)
      {
        throw new InvalidOperationException("Mật khẩu lặp lại không khớp.");
      }
      bool result = await _authService.ForgotPassword(req.token, req.email, req.newPassword);
      if (result)
      {
        return true;
      }
      else
      {
        throw new InvalidOperationException("Đổi mật khẩu thất bại.");
      }
    }
  }
}