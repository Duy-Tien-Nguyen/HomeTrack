using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using HomeTrack.Domain.Enum;
using Microsoft.Extensions.Options;

namespace HomeTrack.Infrastructure.Jwt
{
    public class JwtService
    {
        private readonly JwtSetting _jwtSetting;

        public JwtService(IOptions<JwtSetting> jwtOptions)
        {
            _jwtSetting = jwtOptions.Value;
            if (string.IsNullOrEmpty(_jwtSetting?.SecretKey))
            {
                Console.WriteLine("FATAL ERROR in JwtService Constructor: JWT SecretKey is NULL or EMPTY from IOptions.");
                throw new InvalidOperationException("JWT SecretKey is missing in IOptions<JwtSetting>.");
            }
            Console.WriteLine($"DEBUG in JwtService Constructor - SecretKey from IOptions: '{_jwtSetting.SecretKey}'");
        }

        public string GenerateJwtToken(string userId, string email, string role, JwtType jwtType)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role),
            };

            Console.WriteLine($"DEBUG in GenerateJwtToken - Using SecretKey: '{_jwtSetting.SecretKey}' for signing");
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSetting.SecretKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            int expiresInMinutes = jwtType == JwtType.AccessToken
                ? _jwtSetting.AccessExpiresInMinutes
                : _jwtSetting.RefreshExpiresInMinutes;

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(expiresInMinutes),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
