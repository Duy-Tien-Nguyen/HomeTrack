using FluentValidation;
using FluentValidation.AspNetCore;
public static class Validation
{
  public static void ValidateService(this WebApplicationBuilder builder)
  {
    // Auth
    builder.Services.AddValidatorsFromAssemblyContaining<RegisterRequestValidator>();
    builder.Services.AddValidatorsFromAssemblyContaining<SubmitOTPRequestValidator>();

    
    builder.Services.AddFluentValidationAutoValidation();
    builder.Services.AddFluentValidationClientsideAdapters();
  }
}
