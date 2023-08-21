using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TFAuto.Domain.Services.UserPassword;
using TFAuto.Domain.Services.UserPassword.DTO;

namespace TFAuto.WebApp.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("password")]

    public class UserPasswordController : ControllerBase
    {
        private readonly IUserPasswordService _userPasswordService;

        public UserPasswordController(IUserPasswordService userPasswordService)
        {
            _userPasswordService = userPasswordService;
        }

        [HttpPost]
        [Route("forgot-password")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(string))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordRequest request)
        {
            await _userPasswordService.RequestPasswordResetAsync(request);
            return Ok("You may reset your password now.");
        }

        [HttpPost]
        [Route("reset-password")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(string))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> ResetPassword(ResetPasswordRequest request)
        {
            await _userPasswordService.ResetPasswordAsync(request);
            return Ok("Password successfuly reset.");
        }
    }
}