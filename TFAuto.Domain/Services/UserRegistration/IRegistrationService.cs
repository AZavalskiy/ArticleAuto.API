namespace TFAuto.Domain;

public interface IRegistrationService
{
    public Task<UserRegistrationResponseModel> RegisrateUser(UserRegistrationRequestModel userRequest);
}
