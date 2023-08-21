using TFAuto.Domain.Services.UserPassword.DTO;

namespace TFAuto.Domain.Services.UserPassword
{
    public interface IUserPasswordService
    {
        Task<bool> RequestPasswordResetAsync(ForgotPasswordRequest request);

        Task<bool> ResetPasswordAsync(ResetPasswordRequest request);
    }
}