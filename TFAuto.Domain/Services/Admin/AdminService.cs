using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Extensions;
using SendGrid.Helpers.Errors.Model;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Text;
using TFAuto.DAL.Entities;
using TFAuto.Domain.ExtensionMethods;
using TFAuto.Domain.Services.Admin.DTO.Request;
using TFAuto.Domain.Services.Admin.DTO.Response;
using User = TFAuto.TFAuto.DAL.Entities.User;

namespace TFAuto.Domain.Services.Admin
{
    public class AdminService : IAdminService
    {
        private readonly IRepository<User> _repositoryUser;
        private readonly IRepository<Role> _repositoryRole;

        public AdminService(
            IRepository<User> repositoryUser,
            IRepository<Role> repositoryRole)
        {
            _repositoryUser = repositoryUser;
            _repositoryRole = repositoryRole;

        }

        public async ValueTask<GetUserResponse> GetUserByUserNameOrEmailAsync(string query)
        {
            var userByName = await _repositoryUser.GetAsync(t => t.UserName == query).FirstOrDefaultAsync();

            var userByEmail = await _repositoryUser.GetAsync(t => t.Email == query).FirstOrDefaultAsync();

            if (userByName != null)
            {
                var role = await _repositoryRole.GetAsync(userByName.RoleId, "Role");

                if (role is null)
                    throw new NotFoundException(ErrorMessages.ROLES_NOT_FOUND);

                var userInfo = new GetUserResponse
                {
                    UserId = userByName.Id,
                    UserName = userByName.UserName,
                    Email = userByName.Email,
                    RoleName = role.RoleName
                };

                return userInfo;
            }

            else if (userByEmail != null)
            {
                var role = await _repositoryRole.GetAsync(userByEmail.RoleId, "Role");

                if (role is null)
                    throw new NotFoundException(ErrorMessages.ROLES_NOT_FOUND);

                var userInfo = new GetUserResponse
                {
                    UserId = userByEmail.Id,
                    UserName = userByEmail.UserName,
                    Email = userByEmail.Email,
                    RoleName = role.RoleName
                };

                return userInfo;
            }

            throw new NotFoundException(ErrorMessages.USER_NOT_FOUND);
        }

        public async ValueTask<GetUserResponse> GetUserByEmailAsync(string email)
        {
            var user = await _repositoryUser.GetAsync(t => t.Email == email).FirstOrDefaultAsync();

            if (user is null)
                throw new ValidationException(ErrorMessages.USER_NOT_FOUND);

            var role = await _repositoryRole.GetAsync(user.RoleId, nameof(Role));

            if (role is null)
                throw new NotFoundException(ErrorMessages.ROLE_NOT_FOUND);

            var userInfo = new GetUserResponse
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                RoleName = role.RoleName
            };

            return userInfo;
        }

        public async ValueTask<GetUserResponse> ChangeUserRoleAsync(Guid userId, string userNewRole)
        {
            var user = await _repositoryUser.GetAsync(t => t.Id == userId.ToString()).FirstOrDefaultAsync();
            if (user is null)
                throw new ValidationException(ErrorMessages.USER_NOT_FOUND);

            var currentUserRole = await _repositoryRole.GetAsync(user.RoleId, nameof(Role));
            if (currentUserRole is null)
                throw new NotFoundException(ErrorMessages.ROLE_NOT_FOUND);

            var role = await _repositoryRole.GetAsync(t => t.RoleName == userNewRole).FirstOrDefaultAsync();
            if (role is null)
                throw new NotFoundException(ErrorMessages.ROLE_NOT_FOUND);

            if (currentUserRole == role)
                throw new Exception(ErrorMessages.ROLES_ARE_EQUAL);

            user.RoleId = role.Id;
            await _repositoryUser.UpdateAsync(user);

            var userInfo = new GetUserResponse
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                RoleName = role.RoleName
            };

            return userInfo;
        }

        public async ValueTask DeleteUserAsync(Guid userId)
        {
            var user = await _repositoryUser.GetAsync(t => t.Id == userId.ToString()).FirstOrDefaultAsync();

            if (user is null)
                throw new ValidationException(ErrorMessages.USER_NOT_FOUND);

            await _repositoryUser.DeleteAsync(user);
        }

        public async ValueTask<GetAllUsersResponse> GetAllUsersAsync(GetUsersPaginationRequest paginationRequest)
        {
            const int PAGINATION_SKIP_MIN_LIMIT = 0;
            const int PAGINATION_TAKE_MIN_LIMIT = 1;

            if (paginationRequest.Skip < PAGINATION_SKIP_MIN_LIMIT || paginationRequest.Take < PAGINATION_TAKE_MIN_LIMIT)
                throw new Exception(ErrorMessages.PAGE_NOT_EXISTS);

            string queryUsers = await BuildQuery(paginationRequest);
            var users = await _repositoryUser.GetByQueryAsync(queryUsers);

            if (users == null)
                throw new NotFoundException(ErrorMessages.USERS_NOT_FOUND);

            var totalItems = users.Count();

            if (totalItems <= paginationRequest.Skip)
                throw new NotFoundException(ErrorMessages.USERS_NOT_FOUND);

            if ((totalItems - paginationRequest.Skip) < paginationRequest.Take)
                paginationRequest.Take = (totalItems - paginationRequest.Skip);

            var userList = users
                .Skip(paginationRequest.Skip)
                .Take(paginationRequest.Take)
                .Select(async user => await ConvertGetUserResponse(user))
                .WhenAll()
                .ToList();

            var allUsersResponse = new GetAllUsersResponse()
            {
                TotalItems = totalItems,
                Skip = paginationRequest.Skip,
                Take = paginationRequest.Take,
                OrderBy = paginationRequest.SortBy,
                Users = userList
            };

            return allUsersResponse;
        }

        private async ValueTask<string> BuildQuery(GetUsersPaginationRequest paginationRequest)
        {
            string baseQuery = $"SELECT * FROM c WHERE c.type = \"{nameof(User)}\"";

            StringBuilder queryBuilder = new(baseQuery);

            if (paginationRequest.SortBy.ToString() == nameof(SortOrderUsers.UserNameAscending))
            {
                queryBuilder.Append($" ORDER BY c.{nameof(User.UserName)} ASC");
            }

            else if (paginationRequest.SortBy.ToString() == nameof(SortOrderUsers.UserNameDescending))
            {
                queryBuilder.Append($" ORDER BY c.{nameof(User.UserName)} DESC");
            }

            else if (paginationRequest.SortBy.ToString() == nameof(SortOrderUsers.ByRoleAscending))
            {
                queryBuilder.Append($" ORDER BY c.{nameof(User.RoleId)} ASC");
            }

            else if (paginationRequest.SortBy.ToString() == nameof(SortOrderUsers.ByRoleDescending))
            {
                queryBuilder.Append($" ORDER BY c.{nameof(User.RoleId)} DESC");
            }

            return queryBuilder.ToString();
        }

        private async ValueTask<GetUserResponse> ConvertGetUserResponse(User user)
        {
            var role = await _repositoryRole.GetAsync(user.RoleId, nameof(Role));

            if (role is null)
                throw new NotFoundException(ErrorMessages.ROLES_NOT_FOUND);

            var userResponse = new GetUserResponse
            {
                UserId = user.Id,
                UserName = user.UserName,
                Email = user.Email,
                RoleName = role.RoleName
            };

            return userResponse;
        }
    }
}