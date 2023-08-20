using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TFAuto.Domain;

namespace TFAuto.WebApp;

[ApiController]
[Route("registration")]
public class RegistrationController : ControllerBase
{
    private readonly IRegistrationService _registrationServics;

    public RegistrationController(IRegistrationService registrationServics)
    {
        _registrationServics = registrationServics;
    }

    [HttpPost("users")]
    [SwaggerOperation(
     Summary = "Registers user",
     Description = "Returns saved user with user ID",
     Tags = new[] { "Registration" }
    )]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(UserRegistrationResponseModel))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<UserRegistrationResponseModel>> RegisrateUser([FromBody] UserRegistrationRequestModel userRequest)
    {
        var userResponse = await _registrationServics.RegisrateUser(userRequest);
        return Ok(userResponse);
    }
}
