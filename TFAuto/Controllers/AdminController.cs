using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using TFAuto.DAL.Constant;
using TFAuto.Domain.Services.Admin;
using TFAuto.Domain.Services.Admin.DTO.Request;
using TFAuto.Domain.Services.Admin.DTO.Response;

namespace TFAuto.WebApp.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("admin")]
    [Authorize(Policy = PermissionId.MANAGE_USERS)]

    public class AdminController : ControllerBase
    {
        private readonly IAdminService _adminService;

        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [HttpGet("users/search")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(GetUserResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async ValueTask<ActionResult<GetUserResponse>> GetUserByUserNameOrEmailAsync([Required] string userNameOrEmail)
        {
            var user = await _adminService.GetUserByUserNameOrEmailAsync(userNameOrEmail);
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

        [HttpPut("users/{userId:Guid}")]
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