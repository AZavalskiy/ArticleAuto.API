using static TFAuto.Domain.JWTService;

namespace TFAuto.Domain;

public class LoginResponse
{
    public string UserId { get; set; }
    public Token TokenModel { get; set; }
}
