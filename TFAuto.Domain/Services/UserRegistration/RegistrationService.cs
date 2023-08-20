using AutoMapper;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Extensions;
using System.ComponentModel.DataAnnotations;
using TFAuto.DAL.Constant;
using TFAuto.DAL.Entities;
using TFAuto.TFAuto.DAL.Entities;

namespace TFAuto.Domain;

public class RegistrationService : IRegistrationService
{
    private readonly IRepository<User> _repositoryUser;
    private readonly IRepository<Role> _repositoryRole;
    private readonly IMapper _mapper;

    public RegistrationService(IRepository<User> repositoryUser, IRepository<Role> repositoryRole, IMapper mapper)
    {
        _repositoryUser = repositoryUser;
        _repositoryRole = repositoryRole;
        _mapper = mapper;
    }
    public async ValueTask<UserRegistrationResponseModel> RegisrateUser(UserRegistrationRequestModel userRequest)
    {
        var userExistsByEmail = await _repositoryUser.ExistsAsync(c => c.Email == userRequest.Email);
        if (userExistsByEmail)
        {
            throw new ValidationException(ErrorMessages.USER_EXISTS_BY_EMAIL);
        }
        User user = _mapper.Map<User>(userRequest);
        user.RoleId = RoleId.USER;
        var usersRole = await _repositoryRole.GetAsync(c => c.Id == RoleId.USER).FirstOrDefaultAsync();
        user.PermissionIds = usersRole.PermissionIds;
        User dataUser = await _repositoryUser.CreateAsync(user);
        UserRegistrationResponseModel responseUser = _mapper.Map<UserRegistrationResponseModel>(dataUser);
        return responseUser;
    }
}
