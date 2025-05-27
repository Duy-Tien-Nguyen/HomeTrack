using FluentValidation;
public class SubmitOTPRequest
{
  public required string Token { get; set; }
  public required string Email { get; set; }
}