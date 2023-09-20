using TFAuto.DAL.Constant;

namespace TFAuto.Domain.Services.ArticlePage.DTO.Response;

public class GetAuthorResponse
{
    public string UserName { get; set; }

    public string Email { get; set; }

    public int ReceivedLikes { get; set; }

    public string RoleName { get; set; } = RoleNames.AUTHOR;

    public string RoleId { get; set; }
}
