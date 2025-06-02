namespace HomeTrack.Application.Interface
{
    public interface IEmailService
    {
        Task SendOTPEmail(string email, string token);
    }
}