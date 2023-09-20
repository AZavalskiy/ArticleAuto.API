namespace TFAuto.Domain.Services.ArticlePage.DTO.Response;

public class GetTopTagsResponse : BasePagination
{
    public List<TagResponse> Tags { get; set; } = new();
}
