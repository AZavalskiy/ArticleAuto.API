﻿using TFAuto.Domain.Services.UserPassword.DTO;

namespace TFAuto.Domain.Services.UserPassword
{
    public interface IUserPasswordService
    {
        ValueTask<ForgotPasswordResponse> RequestPasswordResetAsync(ForgotPasswordRequest request);

        ValueTask<ResetPasswordResponse> ResetPasswordAsync(ResetPasswordRequest request);
    }
}