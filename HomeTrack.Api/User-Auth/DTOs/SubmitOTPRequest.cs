using FluentValidation;
namespace HomeTrack.Api.Request
{
  public class SubmitOTPRequest
  {
    public required string Token { get; set; }
    public required string Email { get; set; }
  }
}