using HomeTrack.Application.Interface;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace HomeTrack.Application.Services
{
    public class EmailService : IEmailService
    {
        private readonly string _templatePath;
        private readonly ILogger<EmailService> _logger;
        private readonly IConfiguration _config;

        public EmailService(IWebHostEnvironment env, ILogger<EmailService> logger, IConfiguration config)
        {
            _logger = logger;
            _config = config;

            _templatePath = Path.Combine(env.ContentRootPath, "EmailTemplates");
        }

        public async Task SendOTPEmail(string email, string token)
        {
            try
            {

                var templateFile = Path.Combine(_templatePath, "OTPEmailTemplate.html");

                if (!File.Exists(templateFile))
                {
                    _logger.LogError("Email template not found at: {Path}", templateFile);
                    return;
                }

                string otpExpiryString = _config.GetValue<string>("OtpSettings:ExpireTimeInMinutes") ?? "10";
                int expirationMinutes;

                if (!int.TryParse(otpExpiryString, out expirationMinutes))
                {
                    expirationMinutes = 10;
                }
                string body = await File.ReadAllTextAsync(templateFile);
                body = body.Replace("{{OTP}}", token)
                           .Replace("{{ExpirationMinutes}}", expirationMinutes.ToString());

                var message = new MimeMessage();
                message.From.Add(MailboxAddress.Parse(_config["EmailSettings:From"] ?? throw new InvalidOperationException("EmailSettings:From is missing in configuration.")));
                message.To.Add(MailboxAddress.Parse(email));
                message.Subject = "Your HomeTrack OTP Code";

                message.Body = new TextPart(TextFormat.Html)
                {
                    Text = body
                };

                using var client = new SmtpClient();

                string smtpHost = _config["EmailSettings:SmtpHost"] ?? throw new InvalidOperationException("EmailSettings:SmtpHost is missing in configuration.");
                int smtpPort = int.Parse(_config["EmailSettings:SmtpPort"] ?? "587");
                string smtpUser = _config["EmailSettings:Username"] ?? throw new InvalidOperationException("EmailSettings:Username is missing in configuration.");
                string smtpPass = _config["EmailSettings:Password"] ?? throw new InvalidOperationException("EmailSettings:Password is missing in configuration.");

                await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.Auto);
                await client.AuthenticateAsync(smtpUser, smtpPass);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("OTP email sent to {Email}", email);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send OTP email to {Email}", email);
            }
        }
    }
}
