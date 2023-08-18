namespace TFAuto.Domain.Services.PasswordReset
{
    public interface IPasswordResetService
    {
        ValueTask<bool> RequestPasswordResetAsync(string email);

        ValueTask<bool> ResetPasswordAsync(string resetToken, string newPassword);
    }
}