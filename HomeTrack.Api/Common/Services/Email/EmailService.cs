using HomeTrack.Application.Interface; // Đảm bảo interface này tồn tại và đúng
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
            _config = config; // Gán IConfiguration

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

                string otpExpiryString = _config.GetValue<string>("OtpSettings:ExpireTimeInMinutes");

                if (!int.TryParse(otpExpiryString, out int expirationMinutes))
                {
                    _logger.LogWarning("OtpSettings:ExpireTimeInMinutes not found or invalid. Using default 10 minutes.");
                    expirationMinutes = 10; 
                }

                string body = await File.ReadAllTextAsync(templateFile);
                body = body.Replace("{{OTP}}", token)
                           .Replace("{{ExpirationMinutes}}", expirationMinutes.ToString());

                var message = new MimeMessage();

                // Đọc từ section "EmailSettings"
                message.From.Add(MailboxAddress.Parse(_config["EmailSettings:From"] ?? "noreply@yourdomain.com"));
                message.To.Add(MailboxAddress.Parse(email));
                message.Subject = "Your HomeTrack OTP Code";

                message.Body = new TextPart(TextFormat.Html)
                {
                    Text = body
                };

                using var client = new SmtpClient();

                // Đọc từ section "EmailSettings"
                string smtpHost = _config["EmailSettings:SmtpHost"];
                // Chuyển đổi port từ string sang int
                if (!int.TryParse(_config["EmailSettings:SmtpPort"], out int smtpPort)) {
                    _logger.LogError("EmailSettings:SmtpPort is missing or not a valid number. Using default 587.");
                    smtpPort = 587; // Giá trị mặc định nếu có lỗi
                }
                string smtpUser = _config["EmailSettings:Username"];
                string smtpPass = _config["EmailSettings:Password"];

                if (string.IsNullOrEmpty(smtpHost) || string.IsNullOrEmpty(smtpUser) || string.IsNullOrEmpty(smtpPass))
                {
                    _logger.LogError("SMTP configuration (Host, Username, or Password) is missing in EmailSettings.");
                    return; // Hoặc throw exception
                }

                SecureSocketOptions socketOptions = smtpPort == 465 ? SecureSocketOptions.SslOnConnect : SecureSocketOptions.StartTlsWhenAvailable;
                if (smtpPort == 465)
                {
                     _logger.LogInformation("Connecting to SMTP server {SmtpHost}:{SmtpPort} using SSL/TLS.", smtpHost, smtpPort);
                     await client.ConnectAsync(smtpHost, smtpPort, true);
                }
                else
                {
                    _logger.LogInformation("Connecting to SMTP server {SmtpHost}:{SmtpPort} using StartTlsWhenAvailable.", smtpHost, smtpPort);
                    await client.ConnectAsync(smtpHost, smtpPort, SecureSocketOptions.StartTlsWhenAvailable);
                }

                await client.AuthenticateAsync(smtpUser, smtpPass);
                await client.SendAsync(message);
                await client.DisconnectAsync(true);

                _logger.LogInformation("OTP email sent to {Email}", email);
            }
            catch (AuthenticationException authEx)
            {
                _logger.LogError(authEx, "SMTP Authentication failed for user {SmtpUser}. Check username/password or 'less secure app access' / 'app password' for Gmail.", _config["EmailSettings:Username"]);
            }
            catch (SmtpCommandException smtpEx)
            {
                 _logger.LogError(smtpEx, "SMTP command failed. Status Code: {StatusCode}, Message: {ErrorMessage}", smtpEx.StatusCode, smtpEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send OTP email to {Email}", email);
            }
        }
    }
}