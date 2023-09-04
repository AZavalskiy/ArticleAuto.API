using System.ComponentModel.DataAnnotations;

namespace TFAuto.Domain.Services.UserPassword.DTO
{
    public class ForgotPasswordRequest
    {
        [Required]
        public string Email { get; set; }
    }
}