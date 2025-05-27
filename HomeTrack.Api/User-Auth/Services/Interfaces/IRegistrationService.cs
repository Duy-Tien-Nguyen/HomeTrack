namespace HomeTrack.Application.Interface
{
  public interface IRegistrationService
  {
    Task RegisterAsync(string email, string password, string FirstName, string LastName);
    Task<bool> SubmitOTPAsync(string token);
  }
}