using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using TFAuto.Domain.Services.UserInfo;
using TFAuto.Domain.Services.UserInfo.DTO;
using TFAuto.Domain.Services.UserUpdate;
using TFAuto.Domain.Services.UserUpdate.DTO;

namespace TFAuto.WebApp.Controllers
{
    [ApiController]
    [Route("users")]
    [Authorize]

    public class UserController : ControllerBase
    {
        private readonly IUserInfoService _userInfoService;
        private readonly IUserUpdateInfoService _userUpdateInfoService;

        public UserController(IUserInfoService userInfoService, IUserUpdateInfoService userUpdateInfoService)
        {
            _userInfoService = userInfoService;
            _userUpdateInfoService = userUpdateInfoService;
        }

        [HttpGet("{userId:Guid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(InfoUserResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async ValueTask<ActionResult<InfoUserResponse>> GetUserInfo([Required] Guid userId)
        {
            var user = await _userInfoService.GetUserInfo(userId);
            return Ok(user);
        }

        [HttpPut("{userId:Guid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(UpdateUserInfoResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async ValueTask<ActionResult<UpdateUserInfoResponse>> UpdateUserInfo([Required] Guid userId, [FromQuery] UserUpdateInfoRequest updateInfo)
        {
            var user = await _userUpdateInfoService.UpdateUserInfo(userId, updateInfo);
            return Ok(user);
        }
    }
}
