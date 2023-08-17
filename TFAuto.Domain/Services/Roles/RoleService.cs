using AutoMapper;
using Microsoft.Azure.CosmosRepository;
using System.ComponentModel.DataAnnotations;
using TFAuto.DAL.Entities;
using TFAuto.Domain.Services.Roles.DTO;

namespace TFAuto.Domain.Services.Roles
{
    public class RoleService : IRoleService
    {
        private readonly IRepository<Role> _roleRepository;
        private readonly IMapper _mapper;

        public RoleService(IRepository<Role> roleRepository, IMapper mapper)
        {
            _roleRepository = roleRepository;
            _mapper = mapper;
        }

        public async ValueTask<IEnumerable<RoleListResponse>> GetRolesAsync()
        {
            var roleList = await _roleRepository.GetAsync(t => t.Type == "Role");

            if (roleList == null)
                throw new ValidationException(ErrorMessages.ROLES_NOT_FOUND);

            var roleExistsList = _mapper.Map<IEnumerable<RoleListResponse>>(roleList);

            return roleExistsList;
        }

        public async ValueTask<RoleCreateResponse> AddRoleAsync(RoleCreateRequest newRole)
        {
            var role = await _roleRepository.GetAsync(t => t.RoleName == newRole.RoleName);

            if (role.Any())
                throw new ValidationException(ErrorMessages.ROLE_ALREADY_EXISTS);

            var roleMapped = _mapper.Map<Role>(newRole);
            var result = await _roleRepository.CreateAsync(roleMapped);
            var roleNameNew = _mapper.Map<RoleCreateResponse>(roleMapped);

            return roleNameNew;
        }

        public async ValueTask<RoleUpdateResponse> UpdateRoleAsync(string id, RoleUpdateRequest updatedRole)
        {
            var role = await _roleRepository.TryGetAsync(id, nameof(Role));

            if (role == null)
                throw new ValidationException(ErrorMessages.ROLE_NOT_FOUND);

            role.RoleName = updatedRole.RoleName;
            await _roleRepository.UpdateAsync(role);
            var roleUpdatedName = _mapper.Map<RoleUpdateResponse>(role);

            return roleUpdatedName;
        }

        public async ValueTask DeleteRoleAsync(string id)
        {
            var role = await _roleRepository.TryGetAsync(id, nameof(Role));

            if (role == null)
                throw new ValidationException(ErrorMessages.ROLE_NOT_FOUND);

            await _roleRepository.DeleteAsync(role);
        }
    }
}