using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TFAuto.Domain.Services.PasswordReset;
using TFAuto.Domain.Services.PasswordReset.DTO;
using TFAuto.Domain.Services.ResetPassword;
using TFAuto.Domain.Services.Roles;
using TFAuto.Domain.Services.Roles.DTO;

namespace TFAuto.WebApp.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("forgot-pass")]

    public class ForgotPasswordController : ControllerBase
    {
        private readonly PasswordResetService _passwordResetService;

        public ForgotPasswordController(PasswordResetService passwordResetService)
        {
            _passwordResetService = passwordResetService;
        }

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(string))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]

        public async ValueTask<ActionResult> ForgotPassword(ForgotPasswordRequest model)
        {
            var email = await _passwordResetService.RequestPasswordResetAsync(model.Email);
            return Ok(email);
        }
    }
}
