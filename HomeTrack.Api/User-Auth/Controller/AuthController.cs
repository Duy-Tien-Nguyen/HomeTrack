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
    public AccessTokenString GetAccessToken()
    {
      return _authService.GetAccessToken();
    }

    [Authorize]
    [HttpPatch("reset_password")]
    public async Task<bool> ResetPassword([FromBody] ResetPasswordRequest req)
    {
      var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

      if (string.IsNullOrEmpty(userIdString) || !int.TryParse(userIdString, out int userId))
      {
         throw new InvalidOperationException("Không thể xác định người dùng từ token.");
      }

      return await _authService.ResetPassword(userId, req.newPassword);
    }

    [HttpPatch("forgot_password")]
    public async Task<bool> ForgetPassword([FromBody] ForgetPasswordRequest req)
    {
      return await _authService.ForgotPassword(req);
    }
  }
}