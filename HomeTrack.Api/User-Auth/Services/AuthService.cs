using HomeTrack.Api.Request;
using HomeTrack.Application.Interface;
using HomeTrack.Domain;
using HomeTrack.Infrastructure.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Sprache;
namespace HomeTrack.Application.Services
{
  public class AuthService : IAuthService
  {
    private readonly IUserRepository _userRepo;
    private readonly IPasswordHasher<User> _passwordHasher;

    private readonly JwtService _jwtService;
    private readonly ITokenService _tokenService;

    public AuthService(IUserRepository userRepo,
    IOptions<JwtSetting> jwtSetting,
    IPasswordHasher<User> passwordHasher,
    JwtService jwtService,
    ITokenService tokenService)
    {
      _jwtService = jwtService;
      _tokenService = tokenService;
      _passwordHasher = passwordHasher;
      _userRepo = userRepo;
    }

    public async Task<LoginResponseDto> LoginAsync(LoginRequest req)
    {
      var user = await _userRepo.GetByEmailAsync(req.email);
      if (user == null)
      {
        return new LoginResponseDto { IsSuccess = false, ErrorMessage = "Email hoặc mật khẩu không đúng." };
      }

      if (user.Status != Domain.Enum.UserStatus.Active)
      {
        return new LoginResponseDto { IsSuccess = false, ErrorMessage = "Tài khoản của bạn chưa được tạo thành công hoặc đã bị khóa." };
      }

      if (_passwordHasher.VerifyHashedPassword(user, user.Password, req.password) == PasswordVerificationResult.Failed)
      {
        return new LoginResponseDto { IsSuccess = false, ErrorMessage = "Email hoặc mật khẩu không đúng." };
      }

      var accessToken = _jwtService.GenerateJwtToken(user.Id.ToString(), user.Email, user.Role.ToString(), Domain.Enum.JwtType.AccessToken);
      var refreshToken = _jwtService.GenerateJwtToken(user.Id.ToString(), user.Email, user.Role.ToString(), Domain.Enum.JwtType.RefreshToken);

      user.RefreshToken = refreshToken;
      await _userRepo.SaveChangesAsync();
      return new LoginResponseDto
      {
        IsSuccess = true,
        AccessToken = accessToken,
        RefreshToken = refreshToken,
        User = new UserDto // Map từ User domain model sang UserDto
        {
          Id = user.Id,
          Email = user.Email,
          FirstName = user.FirstName,
          LastName = user.LastName,
          Role = user.Role.ToString()
        }
      };
    }

    public async Task<bool> LogoutAsync(int userId)
    {
      var user = await _userRepo.GetByIdAsync(userId);
      if (user != null)
      {
        user.RefreshToken = string.Empty;
        await _userRepo.SaveChangesAsync();
        return true;
      }
      return false;
    }

    public async Task<AccessTokenString> GetAccessToken(string userId, string email, string role)
    {
      var user = await _userRepo.GetByEmailAsync(email);
      if (user != null && !(user.RefreshToken == "" || user.RefreshToken == null))
      {
        var token = _jwtService.GenerateJwtToken(userId, email, role, Domain.Enum.JwtType.AccessToken);
        return new AccessTokenString { accessToken = token };
      }
      else
      {
        throw new InvalidOperationException("Người dùng không tồn tại.");
      }
      
    }

    public async Task<bool> ResetPassword(int userId, string newPassword)
    {
      var user = await _userRepo.GetByIdAsync(userId);
      if (user != null)
      {
        user.Password = _passwordHasher.HashPassword(user, newPassword);
        await _userRepo.SaveChangesAsync();
        return true;
      }
      else
      {
        throw new InvalidOperationException("Người dùng không tồn tại.");
      }
    }

    public Task<bool> ForgotPassword(string token, string email, string newPassword)
    {
      var userTask = _userRepo.GetByEmailAsync(email);
      return userTask.ContinueWith(async t =>
      {
        var user = await t;
        if (user != null)
        {
          bool isTokenValid = await _tokenService.VerifyTokenAsync(user.Id, token);
          if (isTokenValid)
          {
            user.Password = _passwordHasher.HashPassword(user, newPassword);
            await _userRepo.SaveChangesAsync();
            return true;
          }
          else
          {
            throw new InvalidOperationException("Token không hợp lệ.");
          }
        }
        else
        {
          throw new InvalidOperationException("Người dùng không tồn tại.");
        }
      }).Unwrap();
    }
  }
}