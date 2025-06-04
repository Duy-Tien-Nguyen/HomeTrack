using HomeTrack.Api.Request;
using HomeTrack.Application.Interface;
using HomeTrack.Domain;
using HomeTrack.Infrastructure.Jwt;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Sprache;
using HomeTrack.Api.Models.Entities;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace HomeTrack.Application.Services
{
  public class AuthService : IAuthService
  {
    private readonly IUserRepository _userRepo;
    private readonly IPasswordHasher<HomeTrack.Api.Models.Entities.User> _passwordHasher;

    private readonly JwtService _jwtService;
    private readonly ITokenService _tokenService;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(IUserRepository userRepo,
    IOptions<JwtSetting> jwtSetting,
    IPasswordHasher<HomeTrack.Api.Models.Entities.User> passwordHasher,
    JwtService jwtService,
    ITokenService tokenService,
    IHttpContextAccessor httpContextAccessor)
    {
      _jwtService = jwtService;
      _tokenService = tokenService;
      _passwordHasher = passwordHasher;
      _userRepo = userRepo;
      _httpContextAccessor = httpContextAccessor;
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

      if (_passwordHasher.VerifyHashedPassword(user, user.Password!, req.password) == PasswordVerificationResult.Failed)
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
        User = new UserDto
        {
          Id = user.Id,
          Email = user.Email,
          FirstName = user.Firstname,
          LastName = user.Lastname,
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

    public AccessTokenString GetAccessToken()
    {
      var httpContext = _httpContextAccessor.HttpContext;

      if (httpContext?.User == null)
      {
          throw new InvalidOperationException("HttpContext hoặc User không khả dụng. Đảm bảo request được xác thực.");
      }

      var userId = httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
      var email = httpContext.User.FindFirst(ClaimTypes.Email)?.Value;
      var role = httpContext.User.FindFirst(ClaimTypes.Role)?.Value;

       if (string.IsNullOrEmpty(userId) || string.IsNullOrEmpty(email) || string.IsNullOrEmpty(role))
      {
         throw new InvalidOperationException("Thông tin người dùng không đầy đủ trong token.");
      }

      var token = _jwtService.GenerateJwtToken(userId, email, role, Domain.Enum.JwtType.AccessToken);
      return new AccessTokenString { accessToken = token };
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

    public async Task<bool> ForgotPassword(ForgetPasswordRequest req)
    {
      if (req.newPassword != req.repeatPassword)
      {
        throw new InvalidOperationException("Mật khẩu lặp lại không khớp.");
      }

      var user = await _userRepo.GetByEmailAsync(req.email);
      if (user == null)
      {
        throw new InvalidOperationException("Người dùng không tồn tại.");
      }

      bool isTokenValid = await _tokenService.VerifyTokenAsync(user.Id, req.token);
      if (isTokenValid)
      {
        user.Password = _passwordHasher.HashPassword(user, req.newPassword);
        await _userRepo.SaveChangesAsync();
        return true;
      }
      else
      {
        throw new InvalidOperationException("Token không hợp lệ hoặc đã hết hạn.");
      }
    }
  }
}