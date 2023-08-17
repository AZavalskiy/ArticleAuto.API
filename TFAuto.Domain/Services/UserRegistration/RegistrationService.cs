using AutoMapper;
using Microsoft.Azure.CosmosRepository;
using System.ComponentModel.DataAnnotations;
using TFAuto.TFAuto.DAL.Entities;

namespace TFAuto.Domain;

public class RegistrationService : IRegistrationService
{
    private readonly IRepository<User> _repositoryUser;
    private readonly IMapper _mapper;

    public RegistrationService(IRepository<User> repositoryUser, IMapper mapper)
    {
        _repositoryUser = repositoryUser;
        _mapper = mapper;
    }

    public async Task<UserRegistrationResponseModel> RegisrateUser(UserRegistrationRequestModel userRequest)
    {
        var userExistsByEmail = await _repositoryUser.ExistsAsync(c => c.Email == userRequest.Email);
        if (userExistsByEmail)
        {
            throw new ValidationException(ErrorMessages.USER_EXISTS_BY_EMAIL);
        }
        User user = _mapper.Map<User>(userRequest);
        User dataUser = await _repositoryUser.CreateAsync(user);

        UserRegistrationResponseModel responseUser = _mapper.Map<UserRegistrationResponseModel>(dataUser);
        return responseUser;
    }
}
