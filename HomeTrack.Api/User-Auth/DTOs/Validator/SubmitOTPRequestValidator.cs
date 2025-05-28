using FluentValidation;
using HomeTrack.Api.Request;

public class SubmitOTPRequestValidator : AbstractValidator<SubmitOTPRequest>
{
    public SubmitOTPRequestValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token is required")
            .Length(6).WithMessage("Token must be exactly 6 characters long");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");
    }
}