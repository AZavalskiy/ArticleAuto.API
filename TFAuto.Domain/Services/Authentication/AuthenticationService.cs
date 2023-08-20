using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Extensions;
using System.IdentityModel.Tokens.Jwt;
using TFAuto.TFAuto.DAL.Entities;
using static TFAuto.Domain.JWTService;

namespace TFAuto.Domain;

public class AuthenticationService : IAuthenticationService
{
    private readonly IRepository<User> _repositoryUser;
    private readonly JWTService _jwtService;

    public AuthenticationService(IRepository<User> repositoryUser, JWTService jwtService)
    {
        _repositoryUser = repositoryUser;
        _jwtService = jwtService;
    }
    public async ValueTask<LoginResponse> LoginAsync(LoginRequest loginCredentials)
    {
        var user = await _repositoryUser.GetAsync(c => c.Email == loginCredentials.Email.ToLower()).FirstOrDefaultAsync();
        if (user == null)
        {
            throw new ArgumentException("Invalid credentials");
        }
        var hashedPassword = user.Password;

        if (!BCrypt.Net.BCrypt.Verify(loginCredentials.Password, hashedPassword))
        {
            throw new ArgumentException("Invalid credentials");
        }
        var token = await _jwtService.GenerateTokenMode(user.Id, user.Email);
        return new LoginResponse
        {
            UserId = user.Id,
            TokenModel = token
        };
    }
    public async ValueTask<LoginResponse> GetNewTokensByRefreshAsync(string refreshToken)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var decodedToken = tokenHandler.ReadJwtToken(refreshToken);
        var userIdFromClaims = decodedToken.Claims.FirstOrDefault(c => c.Type == "sub")?.Value;
        var userEmailFromClaims = decodedToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;
        var isRefresh = decodedToken.Claims.FirstOrDefault(c => c.Type == "isRefresh")?.Value;
        if (decodedToken.ValidTo < DateTime.UtcNow || isRefresh == "false")
        {
            throw new ArgumentException("Please enter credentials again");
        }
        var token = await _jwtService.GenerateTokenMode(userIdFromClaims, userEmailFromClaims);
        return new LoginResponse
        {
            UserId = userIdFromClaims,
            TokenModel = token
        };
    }
}
