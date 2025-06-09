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
            services.AddScoped<IPackageRepository, PackageRepository>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<ISystemSettingRepository, SystemSettingRepository>();
            services.AddScoped<IPackageService, PackageService>();
            services.AddScoped<ISubscriptionRepository, SubscriptionRepository>();
            services.AddScoped<ISubscriptionService, SubscriptionService>();
            services.AddScoped<ISystemSettingService, SystemSettingService>();
            services.AddScoped<IItemService, ItemService>();
            services.AddScoped<ISearchService, SearchService>();
            services.AddScoped<IStatisticsService, StatisticsService>();


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

          c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
          {
              Name = "Authorization",
              Type = SecuritySchemeType.Http,
              Scheme = "bearer",
              BearerFormat = "JWT",
              In = ParameterLocation.Header,
              Description = "Nhập token dạng: Bearer {token}"
          });

          c.AddSecurityRequirement(new OpenApiSecurityRequirement
          {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
          });
      });

        }
    }

}
