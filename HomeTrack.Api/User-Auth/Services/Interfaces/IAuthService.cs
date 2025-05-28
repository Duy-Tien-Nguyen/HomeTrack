using HomeTrack.Api.Request;

namespace HomeTrack.Application.Interface
{
  public interface IAuthService
  {
    public Task<LoginResponseDto> LoginAsync(LoginRequest loginRequest);
    public Task LogoutAsync(int userId);
    public Task<string> GetAccessToken(string userId, string email, string role);
  }
}