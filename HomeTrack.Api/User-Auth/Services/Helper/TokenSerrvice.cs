using HomeTrack.Application.Interface;
<<<<<<< HEAD

using System;
using System.Threading.Tasks;
using HomeTrack.Api.Models.Entities; 
using Microsoft.Extensions.Logging; 
=======
using HomeTrack.Domain.Account;
using System;
using System.Threading.Tasks;
>>>>>>> parent of 344a9ee (add Package, Subscription validator for DTO)

namespace HomeTrack.Application.Services
{
    public class TokenService : ITokenService
    {
        // Sử dụng ITokenRepository với ConfirmationToken từ HomeTrack.Api.Models.Entities
        private readonly ITokenRepository _tokenRepo;
        private readonly ILogger<TokenService> _logger;

        public TokenService(ITokenRepository tokenRepo, ILogger<TokenService> logger)
        {
            _tokenRepo = tokenRepo;
            _logger = logger;
        }

        public async Task<string> CreateTokenAsync(int userId)
        {
            Random random = new Random();

            string otp = random.Next(0, 1000000).ToString("D6");

            // Tạo instance từ HomeTrack.Api.Models.Entities.ConfirmationToken
            var confirmation = new HomeTrack.Api.Models.Entities.ConfirmationToken
            {
                UserId = userId,
                Token = otp,
                ExpirationAt = DateTime.UtcNow.AddMinutes(10),
                Used = false
            };

            try
            {
                await _tokenRepo.AddAsync(confirmation);
                await _tokenRepo.SaveChangesAsync();
                _logger.LogInformation("Confirmation token created and saved for UserId {UserId}, Token: {Token}", userId, otp);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException("Failed to create token", ex);
            }
            return otp;
        }

        public async Task<bool> VerifyTokenAsync(int userId, string token)
        {
            try
            {
                // Nhận về Entity từ HomeTrack.Api.Models.Entities.ConfirmationToken
                var tokenData = await _tokenRepo.GetByUserIdAndTokenAsync(userId, token);
                if (tokenData == null || tokenData.Used || tokenData.ExpirationAt < DateTime.UtcNow
                    || tokenData.Token != token) // Thuộc tính Used và Token từ Entity đúng
                    return false;

                await _tokenRepo.MarkAsUsedAsync(tokenData.Id);
                await _tokenRepo.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to verify token for UserId {UserId}", userId);
                throw new InvalidOperationException("Failed to verify token", ex);
            }
        }

        public async Task RevokeAllUserOTP(int userId)
        {
            try
            {
                await _tokenRepo.InvalidateActiveOtpTokensForUserAsync(userId);
                await _tokenRepo.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to revoke OTP tokens for UserId {UserId}", userId);
                throw new InvalidOperationException("Failed to revoke OTP tokens", ex);
            }
        }
    }
}