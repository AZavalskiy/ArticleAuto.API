using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using TFAuto.DAL;
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

    [HttpPost("/users")]
    [SwaggerOperation(
     Summary = "Registers user",
     Description = "Returns saved user with userid",
     Tags = new[] { "Registration" }
    )]
    [SwaggerResponse(200, "Success", typeof(UserRegistrationResponse))]
    [SwaggerResponse(400, "Bad Request")]
    [SwaggerResponse(500, "Internal Server Error")]
    public async Task<ActionResult<UserRegistrationResponse>> RegisrateUser([FromBody] UserRegistrationRequest userRequest)
    {
        var userResponse = await _registrationServics.RegisrateUser(userRequest);
        return Ok(userResponse);
    }
}
