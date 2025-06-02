using System.Data;
using FluentValidation;
using FluentValidation.Validators;
using HomeTrack.Api.Request;

public class LoginRequestValidator : AbstractValidator<LoginRequest>
{
  public LoginRequestValidator()
  {
    RuleFor(x => x.email)
      .NotEmpty().WithMessage("Địa chỉ Email không được để trống.")
      .EmailAddress().WithMessage("Địa chỉ Email không hợp lệ.");

    RuleFor(x => x.password)
      .NotEmpty().WithMessage("Mật khẩu không được để trống")
      .MinimumLength(8).WithMessage("Mật khẩu phải có ít nhất 8 kí tự.");
  }
}

public class ReserPasswordRequestValidator : AbstractValidator<ResetPasswordRequest>
{
  public ReserPasswordRequestValidator()
  {
    RuleFor(x => x.newPassword)
      .NotEmpty().WithMessage("Mật khẩu không được để trống")
      .MinimumLength(8).WithMessage("Mật khẩu phải có ít nhất 8 kí tự.");
  }
}

public class ForgetPasswordRequestValidator : AbstractValidator<ForgetPasswordRequest>
{
  public ForgetPasswordRequestValidator()
  {
    RuleFor(x => x.newPassword)
      .NotEmpty().WithMessage("Mật khẩu không được để trống")
      .MinimumLength(8).WithMessage("Mật khẩu phải có ít nhất 8 kí tự.");

    RuleFor(x => x.email)
      .NotEmpty().WithMessage("Địa chỉ Email không được để trống.")
      .EmailAddress().WithMessage("Địa chỉ Email không hợp lệ.");

    RuleFor(x => x.token)
            .NotEmpty().WithMessage("Mã OTP không hợp lệ.")
            .Length(6).WithMessage("Mã OTP phải có chính xác 6 kí tự.");
  }
}