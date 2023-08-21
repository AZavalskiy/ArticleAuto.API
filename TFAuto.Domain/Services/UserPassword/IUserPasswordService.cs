using TFAuto.Domain.Services.UserPassword.DTO;

namespace TFAuto.Domain.Services.UserPassword
{
    public interface IUserPasswordService
    {
        ValueTask<bool> RequestPasswordResetAsync(ForgotPasswordRequest request);

        ValueTask<bool> ResetPasswordAsync(ResetPasswordRequest request);
    }
}