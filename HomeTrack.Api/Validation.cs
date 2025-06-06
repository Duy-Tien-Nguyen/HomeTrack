using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using HomeTrack.Infrastructure.Jwt;
using HomeTrack.Api.Request;
using HomeTrack.Application.Services;
using System.Security.Claims;

namespace HomeTrack.Application.AcprojSupport
{
    public static class Validation
    {
        public static void ValidateService(this WebApplicationBuilder builder)
        {
            Console.WriteLine("DEBUG: ValidateService method in Validation.cs is being called.");
            // 1. FluentValidation
            builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<SubmitOTPRequestValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<ResendOTPRequestValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<LoginRequestValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<ResetPasswordRequest>();
            builder.Services.AddValidatorsFromAssemblyContaining<ForgetPasswordRequest>();
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddFluentValidationClientsideAdapters();

            // 2. JWT, OTP Settings
            builder.Services.Configure<JwtSetting>(builder.Configuration.GetSection("Jwt"));
            var jwtConfig = builder.Configuration.GetSection("Jwt").Get<JwtSetting>();

            builder.Services.Configure<EmailService>(builder.Configuration.GetSection("OtpSettings"));

            if (string.IsNullOrEmpty(jwtConfig?.SecretKey))
            {
                Console.WriteLine("FATAL ERROR in Validation.cs: JWT SecretKey is NULL or EMPTY from configuration.");
                throw new InvalidOperationException("JWT SecretKey is missing in configuration.");
            }
            Console.WriteLine($"DEBUG in Validation.cs - SecretKey from jwtConfig: '{jwtConfig.SecretKey}'"); 

            // 3. JwtService Singleton
            builder.Services.AddSingleton<JwtService>();

            // 4. Add JWT Authentication
            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.RequireHttpsMetadata = false;
                options.SaveToken = true;
                Console.WriteLine($"DEBUG in AddJwtBearer - Using SecretKey: '{jwtConfig.SecretKey}' for IssuerSigningKey"); 

                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,

                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtConfig.SecretKey)
                    ),
                    ClockSkew = TimeSpan.Zero, // bỏ thời gian trễ mặc định 5 phút
                    RoleClaimType= ClaimTypes.Role
                };
            });
        }
    }
}
