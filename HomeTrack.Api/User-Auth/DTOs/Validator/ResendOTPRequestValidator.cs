using FluentValidation;
using HomeTrack.Api.Request;
public class ResendOTPRequestValidator : AbstractValidator<ResendOTPRequest>
{
  public ResendOTPRequestValidator()
  {
    RuleFor(x => x.email)
      .NotEmpty().WithMessage("Email is required")
      .EmailAddress().WithMessage("Invalid email format");
  }
}