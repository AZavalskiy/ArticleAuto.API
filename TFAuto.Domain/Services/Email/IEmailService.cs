namespace TFAuto.Domain.Services.Email
{
    public interface IEmailService
    {
        ValueTask SendConfirmationEmailAsync(string userEmail, string confirmationLink);

        ValueTask SendPasswordResetEmailAsync(string userEmail, string resetToken, string resetLink);
    }
}
