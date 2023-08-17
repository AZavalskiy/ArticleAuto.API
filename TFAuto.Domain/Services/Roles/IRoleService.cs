using TFAuto.Domain.Services.Roles.DTO;

namespace TFAuto.Domain.Services.Roles
{
    public interface IRoleService
    {
        ValueTask<IEnumerable<RoleListResponse>> GetRolesAsync();

        ValueTask<RoleCreateResponse> AddRoleAsync(RoleCreateRequest newRole);

        ValueTask<RoleUpdateResponse> UpdateRoleAsync(string id, RoleUpdateRequest updatedRole);

        ValueTask DeleteRoleAsync(string id);
    }
}
