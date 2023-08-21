using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Annotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TFAuto.Domain;
using static TFAuto.Domain.JWTService;

namespace TFAuto.WebApp;

[Route("authentication")]
[ApiController]
public class AuthenticationController : ControllerBase
{
    private readonly AuthenticationService _userService;

    public AuthenticationController(AuthenticationService userService)
    {
        _userService = userService;
    }
    [HttpPost("login")]
    [SwaggerOperation(
     Summary = "Login authentication",
     Description = "Logs in with user's credentials and returns access and refresh tokens for authentication"
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(LoginResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<LoginResponse>> LoginAsync([FromBody] LoginRequest loginCredentials)
    {
        var createdLogin = await _userService.LoginAsync(loginCredentials);
        return Ok(createdLogin);
    }

    [HttpPost("refresh-token")]
    [SwaggerOperation(
    Summary = "Get new tokens with Refresh token",
    Description = "Using valid refresh token get new pair of valid Access/Refresh tokens"
   )]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(LoginResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<LoginResponse>> GetNewTokensByRefreshAsync([FromBody] RefreshRequest refreshToken)
    {
        var createdLogin = await _userService.GetNewTokensByRefreshAsync(refreshToken);
        return Ok(createdLogin);
    }
}
