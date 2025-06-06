using HomeTrack.Api.Request;
using System.Threading.Tasks;

namespace HomeTrack.Application.Interface
{
  public interface IAuthService
  {
    public Task<LoginResponseDto> LoginAsync(LoginRequest loginRequest);
    public Task<bool> LogoutAsync(int userId);
    public AccessTokenString GetAccessToken();
    public Task<bool> ForgotPassword(ForgetPasswordRequest req);
    public Task<bool> ResetPassword(int userId, string newPassword);
    public Task<bool> ForgotPassword(string token, string email, string newPassword);
    public Task<UserDto> GetProfile(int userId);
  }
}