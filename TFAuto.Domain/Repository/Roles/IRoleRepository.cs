using TFAuto.Domain.Repository.Roles.DTO;

namespace TFAuto.Domain.Repository.Roles
{
    public interface IRoleRepository
    {
        Task<IEnumerable<RoleListResponse>> GetRolesAsync();

        Task<RoleCreateResponse> AddRoleAsync(RoleCreateRequest newRole);

        Task<RoleUpdateResponse> UpdateRoleAsync(string id, RoleUpdateRequest updatedRole);

        Task DeleteRoleAsync(string id);

    }
}
