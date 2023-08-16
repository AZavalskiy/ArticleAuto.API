using AutoMapper;
using Microsoft.Azure.CosmosRepository;
using TFAuto.DAL.Entities;
using TFAuto.Domain.Repository.Roles.DTO;

namespace TFAuto.Domain.Repository.Roles
{

    public class RoleRepository : IRoleRepository
    {
        private readonly IRepository<Role> _roleRepository;
        private readonly IMapper _mapper;

        public RoleRepository(IRepository<Role> roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async Task<IEnumerable<RoleListResponse>> GetRolesAsync()
        {
            var roleList = await _roleRepository.GetAsync(t => t.Type == "Role");

            if (roleList == null)
                throw new Exception("Roles didn't found.");

            var roleExistsList = _mapper.Map<IEnumerable<RoleListResponse>>(roleList);

            return roleExistsList;
        }

        public async Task<RoleCreateResponse> AddRoleAsync(RoleCreateRequest newRole)
        {
            var role = await _roleRepository.GetAsync(t => t.RoleName == newRole.RoleName);

            if (role.Any())
                throw new Exception("Role already exists");

            var roleMapped = _mapper.Map<Role>(newRole);
            var result = await _roleRepository.CreateAsync(roleMapped);
            var roleNameNew = _mapper.Map<RoleCreateResponse>(roleMapped);

            return roleNameNew;
        }

        public async Task<RoleUpdateResponse> UpdateRoleAsync(string id, RoleUpdateRequest updatedRole)
        {
            var role = await _roleRepository.GetAsync(id, nameof(Role));
            
            if (role == null)
                throw new Exception("Role not found.");

            role.RoleName = updatedRole.RoleName;
            await _roleRepository.UpdateAsync(role);
            var roleUpdatedName = _mapper.Map<RoleUpdateResponse>(role);            

            return roleUpdatedName;
        }

        public async Task DeleteRoleAsync(string id)
        {
            var role = await _roleRepository.GetAsync(id, nameof(Role));

            if (role == null)
                throw new Exception("Role not found.");

            await _roleRepository.DeleteAsync(role);
        }

    }

}