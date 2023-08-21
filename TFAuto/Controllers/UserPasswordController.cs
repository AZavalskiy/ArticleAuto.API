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
        public async ValueTask<ActionResult<ForgotPasswordResponse>> ForgotPassword(ForgotPasswordRequest request)
        {
            var response = await _userPasswordService.RequestPasswordResetAsync(request);
            return Ok(response);
        }

        [HttpPost]
        [Route("reset-password")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(string))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async ValueTask<ActionResult<ResetPasswordResponse>> ResetPassword(ResetPasswordRequest request)
        {
            var response = await _userPasswordService.ResetPasswordAsync(request);
            return Ok(response);
        }
    }
}