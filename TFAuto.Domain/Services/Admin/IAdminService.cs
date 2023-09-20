using TFAuto.Domain.Services.Admin.DTO.Request;
using TFAuto.Domain.Services.Admin.DTO.Response;

namespace TFAuto.Domain.Services.Admin
{
    public interface IAdminService
    {
        ValueTask<GetUserResponse> GetUserByNameAsync(string userName);

        ValueTask<GetUserResponse> GetUserByEmailAsync(string email);

        ValueTask<GetAllUsersResponse> GetAllUsersAsync(GetUsersPaginationRequest paginationRequest);

        ValueTask<GetUserResponse> ChangeUserRoleAsync(Guid userId, string userNewRole);

        ValueTask DeleteUserAsync(Guid userId);
    }
}