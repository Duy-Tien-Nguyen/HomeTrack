using HomeTrack.Application.Interface;
using HomeTrack.Domain;
using HomeTrack.Domain.Enum;
using HomeTrack.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;

namespace HomeTrack.Application.Services
{
    public class RegistrationService : IRegistrationService
    {
        private readonly IUserRepository _userRepo;
        private readonly ITokenService _tokenService;
        private readonly IEmailService _emailService;
        private readonly ILogger<RegistrationService> _logger;
        private readonly IPasswordHasher<User> _passwordHasher;


        public RegistrationService(IUserRepository userRepo, ITokenService tokenService, IEmailService emailService, ILogger<RegistrationService> logger, IPasswordHasher<User> passwordHasher)
        {
            _userRepo = userRepo;
            _tokenService = tokenService;
            _emailService = emailService;
            _logger = logger;
            _passwordHasher = passwordHasher;
        }

        public async Task RegisterAsync(string email, string password, string firstName, string lastName)
        {
            _logger.LogInformation("Starting registration process for {Email}", email);

            // 1. Kiểm tra xem user đã tồn tại chưa (nên có)
            var existingUser = await _userRepo.GetByEmailAsync(email);
            if (existingUser != null)
            {
                _logger.LogWarning("Registration attempt for existing email: {Email}", email);
                throw new InvalidOperationException("Email already registered.");
            }
            var user = new User
            {
                Email = email,
                Password = string.Empty, // Password will be set after hashing
                FirstName = firstName,
                LastName = lastName,
                Role = Role.Basic,
                Status = Status.Pending,
            };
            user.Password = _passwordHasher.HashPassword(user, password);

            await _userRepo.AddAsync(user);
             try
            {
                await _userRepo.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error saving new user {Email} to database.", email);
                throw new InvalidOperationException("Failed to register user. Please try again later.");
            }

            var token = await _tokenService.CreateTokenAsync(user.Id);

            await _emailService.SendOTPEmail(email, token);
            _logger.LogInformation($"Registration initiated for {email}. OTP sent: {token}");
        }

        public async Task<bool> SubmitOTPAsync(string token)
        {
            var userId = await _tokenService.VerifyTokenAsync(token);
            if (userId == null) return false;
            await _userRepo.ActivateUserAsync(userId.Value);
            return true;
        }
    }
}