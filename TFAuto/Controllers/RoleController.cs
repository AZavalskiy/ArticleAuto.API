using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using TFAuto.Domain.Repository.Roles;
using TFAuto.Domain.Repository.Roles.DTO;

namespace TFAuto.Web.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("roles")]

    [SwaggerResponse(StatusCodes.Status200OK,"Success")]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]

    public class RoleController : ControllerBase
    {
        private readonly IRoleRepository _roleRepository;

        public RoleController(IRoleRepository roleRepository)
        {
            _roleRepository = roleRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetRoles()
        {   
            var roles = await _roleRepository.GetRolesAsync();
            return Ok(roles);
        }

        [HttpPost]
        public async Task<IActionResult> AddRole([FromBody] RoleCreateRequest newRole)
        {
            var role = await _roleRepository.AddRoleAsync(newRole);
            return Ok(role);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateRole([Required] string id, [FromBody] RoleUpdateRequest updatedRole)
        {
            var role = await _roleRepository.UpdateRoleAsync(id, updatedRole);
            return Ok(role);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteRole([Required] string id)
        {
            await _roleRepository.DeleteRoleAsync(id);
            return Ok();
        }
    }
}
