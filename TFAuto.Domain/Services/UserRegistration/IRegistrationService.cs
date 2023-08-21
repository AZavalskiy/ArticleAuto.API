namespace TFAuto.Domain;

public interface IRegistrationService
{
    public ValueTask<UserRegistrationResponseModel> RegisrateUser(UserRegistrationRequestModel userRequest);
}
