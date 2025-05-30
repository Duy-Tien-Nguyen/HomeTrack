using HomeTrack.Api.Request;

namespace HomeTrack.Application.Interface
{
  public interface IAuthService
  {
    public Task<LoginResponseDto> LoginAsync(LoginRequest loginRequest);
    public Task<bool> LogoutAsync(int userId);
    public Task<AccessTokenString> GetAccessToken(string userId, string email, string role);
    public Task<bool> ResetPassword(int userId, string newPassword);
    public Task<bool> ForgotPassword(string token, string email, string newPassword);
  }
}