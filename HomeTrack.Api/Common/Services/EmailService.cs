using HomeTrack.Application.Interface;
using MailKit.Net.Smtp;
using MimeKit;


namespace HomeTrack.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _config;

        public EmailService(IConfiguration config)
        {
            _config = config;
        }
        public async Task SendOTPEmail(string email, string token)
        {
            var message = new MimeMessage();
            message.From.Add(MailboxAddress.Parse(Environment.GetEnvironmentVariable("EMAIL_FROM")));
            message.To.Add(MailboxAddress.Parse(email));
            message.Subject = "Your OTP Code";

            message.Body = new TextPart("plain")
            {
                Text = $"Your OTP code is: {token}"
            };

            using var client = new SmtpClient();
            await client.ConnectAsync(
                Environment.GetEnvironmentVariable("SMTP_HOST"),
                int.Parse(Environment.GetEnvironmentVariable("SMTP_PORT")),
                true
            );

            await client.AuthenticateAsync(
                Environment.GetEnvironmentVariable("SMTP_USERNAME"),
                Environment.GetEnvironmentVariable("SMTP_PASSWORD")
            );
            await client.SendAsync(message);
            await client.DisconnectAsync(true);
        }
    }
}
