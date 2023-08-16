using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TFAuto.Domain.Repository.Roles.DTO;

namespace TFAuto.Domain.Repository.Roles
{
    public interface IRoleRepository
    {
        Task<IEnumerable<RoleListDTO>> GetRolesAsync();

        Task<string> AddRoleAsync(RoleCreateDTO newRole);

        Task<string> UpdateRoleAsync(string roleName, RoleUpdateDTO updatedRole);

        Task DeleteRoleAsync(string roleName);
    }
}
