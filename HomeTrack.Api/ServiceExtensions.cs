using Microsoft.OpenApi.Models;
using HomeTrack.Application.Interface;
using HomeTrack.Infrastructure.Repositories;
using HomeTrack.Application.Services;
using Microsoft.AspNetCore.Identity;
using HomeTrack.Domain;

namespace HomeTrack.Application.AcprojSupport
{
  public static class ServiceExtensions
  {
    public static void ConfigureServices(this IServiceCollection services)
    {
      services.AddScoped<IRegistrationService, RegistrationService>();
      services.AddScoped<IUserRepository, UserRepository>();
      services.AddScoped<ITokenService, TokenService>();
      services.AddScoped<ITokenRepository, TokenRepository>();
      services.AddScoped<IEmailService, EmailService>();
      services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();
      services.AddScoped<IEmailService, EmailService>();
      services.AddScoped<IAuthService, AuthService>();


      services.AddControllers();
      services.AddEndpointsApiExplorer();
      services.AddSwaggerGen(c =>
      {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
          Title = "HomeTrack API",
          Version = "v1",
          Description = "API for user registration and OTP verification"
        });

        c.EnableAnnotations();
      });
    }
  }
}
