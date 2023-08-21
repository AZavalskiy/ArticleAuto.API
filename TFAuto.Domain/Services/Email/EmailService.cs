using Microsoft.Extensions.Configuration;
using SendGrid;
using SendGrid.Helpers.Mail;
using TFAuto.Domain.ServiceConfigurations;

namespace TFAuto.Domain.Services.Email
{
    public class EmailService : IEmailService
    {
        private readonly IConfiguration _configuration;

        public EmailService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async ValueTask SendConfirmationEmailAsync(string userEmail, string confirmationLink)
        {
            var subject = "Welcome to TFAuto! Confirm Your Email";
            var body = $"<a href='{confirmationLink}'>Click here to confirm your email</a>";
            await SendEmailAsync(userEmail, subject, body);
        }

        public async ValueTask SendPasswordResetEmailAsync(string userEmail, string resetLink)
        {
            var subject = "TFAuto. Password Reset Request";
            var body = $"<a href='{resetLink}'>Click here to reset your password</a>";
            await SendEmailAsync(userEmail, subject, body);
        }

        private async ValueTask SendEmailAsync(string userEmail, string subject, string body)
        {
            var sendGridSettings = _configuration.GetSection("SendGridSettings").Get<SendGridSettings>();

            var apiKey = sendGridSettings.ApiKey;
            var fromName = sendGridSettings.FromName;
            var fromEmail = sendGridSettings.FromEmail;

            var client = new SendGridClient(apiKey);
            var from = new EmailAddress(fromEmail, fromName);
            var to = new EmailAddress(userEmail);
            var msg = MailHelper.CreateSingleEmail(from, to, subject, null, body);

            var response = await client.SendEmailAsync(msg);

            if (response.StatusCode != System.Net.HttpStatusCode.Accepted)
                throw new Exception("Failed to send email.");
        }
    }
}