using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using TFAuto.Domain.Services.Roles;
using TFAuto.Domain.Services.Roles.DTO;
using TFAuto.WebApp;

namespace TFAuto.Web.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("roles")]       
    
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleRepository;

        public RoleController(IRoleService roleRepository)
        {
            _roleRepository = roleRepository;
        }

        [HttpGet]
        [SwaggerResponse(200, "REQUEST_SUCCESSFUL", typeof(RoleListResponse))]
        [SwaggerResponse(400, "BAD_REQUEST", typeof(ErrorDetails))]
        [SwaggerResponse(500, "INTERNAL_SERVER_ERROR", typeof(ErrorDetails))]
        public async ValueTask<ActionResult<RoleListResponse>> GetRoles()
        {   
            var roles = await _roleRepository.GetRolesAsync();
            return Ok(roles);
        }

        [HttpPost]
        [SwaggerResponse(200, "REQUEST_SUCCESSFUL", typeof(RoleCreateResponse))]
        [SwaggerResponse(400, "BAD_REQUEST", typeof(ErrorDetails))]
        [SwaggerResponse(500, "INTERNAL_SERVER_ERROR", typeof(ErrorDetails))]        
        public async ValueTask<ActionResult<RoleCreateResponse>> AddRole([FromBody] RoleCreateRequest newRole)
        {
            var role = await _roleRepository.AddRoleAsync(newRole);
            return Ok(role);
        }

        [HttpPut("{id}")]
        [SwaggerResponse(200, "REQUEST_SUCCESSFUL", typeof(RoleUpdateResponse))]
        [SwaggerResponse(400, "BAD_REQUEST", typeof(ErrorDetails))]
        [SwaggerResponse(500, "INTERNAL_SERVER_ERROR", typeof(ErrorDetails))]
        public async ValueTask<ActionResult<RoleUpdateResponse>> UpdateRole([Required] string id, [FromBody] RoleUpdateRequest updatedRole)
        {
            var role = await _roleRepository.UpdateRoleAsync(id, updatedRole);
            return Ok(role);
        }

        [HttpDelete("{id}")]        
        [SwaggerResponse(400, "BAD_REQUEST", typeof(ErrorDetails))]
        [SwaggerResponse(500, "INTERNAL_SERVER_ERROR", typeof(ErrorDetails))]
        public async ValueTask<IActionResult> DeleteRole([Required] string id)
        {
            await _roleRepository.DeleteRoleAsync(id);
            return Ok();
        }
    }
}
