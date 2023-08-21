using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TFAuto.Domain.Services.UserPassword.DTO
{
    public class ResetPasswordRequest
    {
        [Required]
        public string Token { get; set; } = string.Empty;

        [Required]
        [DefaultValue("abCdeF!$*159+")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&+=-]).{8,}$", ErrorMessage = ErrorMessages.USER_VALID_PASSWORD)]
        [MinLength(8, ErrorMessage = ErrorMessages.USER_VALID_PASSWORD)]
        public string Password { get; set; }

        [Required]
        [DefaultValue("abCdeF!$*159+")]
        [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&+=-]).{8,}$", ErrorMessage = ErrorMessages.USER_VALID_PASSWORD)]
        [MinLength(8, ErrorMessage = ErrorMessages.USER_VALID_PASSWORD)]
        public string ConfirmPassword { get; set; }
    }
}
