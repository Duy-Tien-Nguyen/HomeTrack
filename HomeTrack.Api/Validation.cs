using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using HomeTrack.Infrastructure.Jwt;

namespace HomeTrack.Application.AcprojSupport
{
    public static class Validation
    {
        public static void ValidateService(this WebApplicationBuilder builder)
        {
            // 1. FluentValidation
            builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<SubmitOTPRequestValidator>();
            builder.Services.AddValidatorsFromAssemblyContaining<ResendOTPRequestValidator>();
            builder.Services.AddFluentValidationAutoValidation();
            builder.Services.AddFluentValidationClientsideAdapters();

            // 2. JWT Settings
            builder.Services.Configure<JwtSetting>(builder.Configuration.GetSection("Jwt"));
            var jwtConfig = builder.Configuration.GetSection("Jwt").Get<JwtSetting>();

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
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateLifetime = true,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfig.SecretKey))
                };
            });
        }
    }
}
