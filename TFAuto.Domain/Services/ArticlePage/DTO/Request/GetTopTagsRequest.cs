using System.ComponentModel;

namespace TFAuto.Domain.Services.ArticlePage.DTO.Request;

public class GetTopTagsRequest
{
    [DefaultValue(0)]
    public int Skip { get; set; }

    [DefaultValue(25)]
    public int Take { get; set; }
}
