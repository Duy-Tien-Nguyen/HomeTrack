using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using HomeTrack.Infrastructure.Jwt;
using System.Text;
using HomeTrack.Domain.Enum;

public class JwtService
{

  public string GenerateJwtToken(string userId, string email, string role, JwtType jwtType)
  {
    var claims = new[]
    {
      new Claim(ClaimTypes.NameIdentifier, userId),
      new Claim(ClaimTypes.Email, email),
      new Claim(ClaimTypes.Role, role),
    };

    var secretKey = Environment.GetEnvironmentVariable("JWT_SECRET_KEY");
    if (string.IsNullOrEmpty(secretKey))
    {
      throw new InvalidOperationException("JWT_SECRET_KEY environment variable is not set.");
    }
    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    // Lấy thời gian hết hạn cảu từng loại Token
    var expiresInMinutesStr = Environment.GetEnvironmentVariable("ACCESS_EXPIRES_IN_MINUTES");
    if (jwtType == JwtType.RefreshToken)
    {
      expiresInMinutesStr = Environment.GetEnvironmentVariable("REFRESH_EXPIRES_IN_MINUTES");
    }

    if (string.IsNullOrEmpty(expiresInMinutesStr) || !int.TryParse(expiresInMinutesStr, out int expiresInMinutes))
    {
      throw new InvalidOperationException("JWT_EXPIRES_IN_MINUTES environment variable is not set or invalid.");
    }
    //Tạo token
    var token = new JwtSecurityToken(
      claims: claims,
      expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
      signingCredentials: creds
    );

    return new JwtSecurityTokenHandler().WriteToken(token);
  }
}