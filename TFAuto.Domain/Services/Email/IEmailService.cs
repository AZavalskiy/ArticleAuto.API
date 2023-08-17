namespace TFAuto.Domain.Services.Email
{
    public interface IEmailService
    {
        Task SendConfirmationEmailAsync(string userEmail, string confirmationLink);

        Task SendPasswordResetEmailAsync(string userEmail, string resetLink);
    }
}
