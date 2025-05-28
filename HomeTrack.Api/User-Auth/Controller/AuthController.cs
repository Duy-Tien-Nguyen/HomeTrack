using Microsoft.AspNetCore.Mvc;
using HomeTrack.Application.Interface;
using HomeTrack.Api.Request;
using System.Security.Claims;

namespace HomeTrack.Api.Controllers
{
  [ApiController]
  [Route("api/auth")]

  public class AuthController : ControllerBase
  {
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
      _authService = authService;
    }

    [HttpPost("login")]
    public async Task<LoginResponseDto> LoginAsync([FromBody] LoginRequest req)
    {
      return await _authService.LoginAsync(req);
    }

    [HttpPost("logout")]
    public async Task Loguot([FromBody] int userId)
    {
      await _authService.LogoutAsync(userId);
    }

    [HttpGet("access_token")]
    public Task<string> GetAccessToken()
    {
      var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var email = User.FindFirst(ClaimTypes.Email)?.Value;
      var role = User.FindFirst(ClaimTypes.Role)?.Value;

      return _authService.GetAccessToken(userId, email, role);
    }
  }
}