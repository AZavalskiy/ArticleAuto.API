using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TFAuto.Domain.Services.PasswordReset.DTO
{
    public class ForgotPasswordRequest
    {
        [Required]        
        public string Email { get; set; }
    }
}
