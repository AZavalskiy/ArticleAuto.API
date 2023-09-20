using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using TFAuto.Domain.Services.Admin;
using TFAuto.Domain.Services.Admin.DTO.Request;
using TFAuto.Domain.Services.Admin.DTO.Response;

namespace TFAuto.WebApp.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("admin")]
    //[Authorize]

    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("user-by-name")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(GetUserResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async ValueTask<ActionResult<GetUserResponse>> GetUserByNameAsync([Required] string userName)
        {
            var user = await _adminService.GetUserByNameAsync(userName);
            return Ok(user);
        }

        [HttpGet("user-by-email")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(GetUserResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async ValueTask<ActionResult<GetUserResponse>> GetUserByEmailAsync([Required] string email)
        {
            var user = await _adminService.GetUserByEmailAsync(email);
            return Ok(user);
        }

        [HttpGet("users")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(GetAllUsersResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async ValueTask<ActionResult<GetAllUsersResponse>> GetAllUsersAsync([FromQuery] GetUsersPaginationRequest paginationRquest)
        {
            var users = await _adminService.GetAllUsersAsync(paginationRquest);
            return Ok(users);
        }

        [HttpPut("user-role")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(GetUserResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async ValueTask<ActionResult<GetUserResponse>> ChangeUserRoleAsync([Required] Guid userId, [Required] string userNewRole)
        {
            var user = await _adminService.ChangeUserRoleAsync(userId, userNewRole);
            return Ok(user);
        }

        [HttpDelete("user/{userId:Guid}")]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async ValueTask<IActionResult> DeleteUserAsync([Required] Guid userId)
        {
            await _adminService.DeleteUserAsync(userId);
            return NoContent();
        }

    }
}