using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Extensions;
using System.IdentityModel.Tokens.Jwt;
using TFAuto.Domain.Services.Authentication.Constants;
using TFAuto.Domain.Services.Authentication.Models.Request;
using TFAuto.TFAuto.DAL.Entities;
namespace TFAuto.Domain.Services.Authentication;

public class AuthenticationService : IAuthenticationService
{
    private readonly IRepository<User> _repositoryUser;
    private readonly JWTService _jwtService;

    public AuthenticationService(IRepository<User> repositoryUser, JWTService jwtService)
    {
        _repositoryUser = repositoryUser;
        _jwtService = jwtService;
    }

    public async ValueTask<LoginResponse> LogInAsync(LoginRequest loginCredentials)
    {
        var user = await _repositoryUser.GetAsync(c => c.Email == loginCredentials.Email.ToLower()).FirstOrDefaultAsync();

        if (user == null)
        {
            throw new ArgumentException(ErrorMessages.LOG_IN_INVALID_CREDENTIALS);
        }

        var hashedPassword = user.Password;

        if (!BCrypt.Net.BCrypt.Verify(loginCredentials.Password, hashedPassword))
        {
            throw new ArgumentException(ErrorMessages.LOG_IN_INVALID_CREDENTIALS);
        }

        var token = await _jwtService.GenerateTokenMode(user.Id, user.Email);
        return new LoginResponse
        {
            UserId = user.Id,
            TokenModel = token
        };
    }

    public async ValueTask<LoginResponse> GetNewTokensByRefreshAsync(RefreshRequest refreshToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var decodedToken = tokenHandler.ReadJwtToken(refreshToken.RefreshToken);
        var userIdFromClaims = decodedToken.Claims.FirstOrDefault(c => c.Type == CustomClaimsType.SUBJECT)?.Value;
        var userEmailFromClaims = decodedToken.Claims.FirstOrDefault(c => c.Type == CustomClaimsType.EMAIL)?.Value;
        var isAccess = decodedToken.Claims.FirstOrDefault(c => c.Type == CustomClaimsType.IS_ACCESS)?.Value;

        if (decodedToken.ValidTo < DateTime.UtcNow || bool.Parse(isAccess) == true)
        {
            throw new ArgumentException(ErrorMessages.LOG_IN_CREDENTIALS_AGAIN);
        }

        var token = await _jwtService.GenerateTokenMode(userIdFromClaims, userEmailFromClaims);
        return new LoginResponse
        {
            UserId = userIdFromClaims,
            TokenModel = token
        };
    }
}
