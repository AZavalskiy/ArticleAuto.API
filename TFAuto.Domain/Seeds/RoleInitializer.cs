using Microsoft.Azure.CosmosRepository;
using TFAuto.DAL.Constant;
using TFAuto.DAL.Entities;

namespace TFAuto.Domain.Seeds
{
    public class RoleInitializer
    {
        private readonly IRepository<Role> _roleRepository;

        public RoleInitializer(IRepository<Role> roleRepository)
        {
            _roleRepository = roleRepository;
        }

        public async Task InitializeRoles()
        {
            string userId = "c591984b-ccbc-484b-90db-01e390ffd141";
            string authorId = "c591984b-ccbc-484b-90db-01e390ffd142";
            string superAdminId = "c591984b-ccbc-484b-90db-01e390ffd143";

            List<Role> roles = new()
            {
                new Role { Id = superAdminId, RoleName = RoleNames.SUPER_ADMIN },
                new Role { Id = authorId, RoleName = RoleNames.AUTHOR },
                new Role { Id = userId, RoleName = RoleNames.USER },
            };

            foreach (var role in roles)
            {
                var existingRole = await _roleRepository.ExistsAsync(role.Id, nameof(Role));

                if (!existingRole)
                {
                    await _roleRepository.CreateAsync(role);
                }
            }

        }
    }
}
