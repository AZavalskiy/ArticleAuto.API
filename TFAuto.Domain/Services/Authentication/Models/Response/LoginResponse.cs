using TFAuto.Domain.Services.Authentication.Models;

namespace TFAuto.Domain;

public class LoginResponse
{
    public string UserId { get; set; }

    public Token TokenModel { get; set; }
}
