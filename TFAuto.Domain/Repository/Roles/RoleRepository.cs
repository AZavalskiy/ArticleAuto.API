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

        public async Task<IEnumerable<RoleListDTO>> GetRolesAsync()
        {
            var roleList = await _roleRepository.GetAsync(t => t.PartitionKey == "Role");

            if (roleList == null)
                throw new Exception("Roles didn't found.");

            var roleExistsList = _mapper.Map<IEnumerable<RoleListDTO>>(roleList);

            return roleExistsList;
        }

        public async Task<string> AddRoleAsync(RoleCreateDTO newRole)
        {
            var role = await _roleRepository.GetAsync(t => t.RoleName == newRole.RoleName);

            if (role.Any())
                throw new Exception("Role already exists");

            await _roleRepository.CreateAsync(role);
            var roleDTO = _mapper.Map<RoleCreateDTO>(role);
            string roleNameNew = roleDTO.RoleName;

            return roleNameNew;
        }

        public async Task<string> UpdateRoleAsync(string roleName, RoleUpdateDTO updatedRole)
        {
            var role = await _roleRepository.GetAsync(roleName);

            if (role == null)
                throw new Exception("Role not found.");

            role.RoleName = updatedRole.RoleName;            
            await _roleRepository.CreateAsync(role);
            var roleDTO = _mapper.Map<RoleUpdateDTO>(role);
            
            return roleDTO.RoleName;
        }

        public async Task DeleteRoleAsync(string roleName)
        {
            var role = await _roleRepository.GetAsync(roleName);

            if (role == null)
                throw new Exception("Role not found.");

            await _roleRepository.DeleteAsync(roleName);
        }

    }

}