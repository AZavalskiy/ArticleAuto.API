namespace TFAuto.Domain.Services.ArticlePage.DTO.Response;

public class GetAllArticlesResponse : BasePaginationRequest
{
    public List<GetArticleResponse> Articles { get; set; } = new();

    public SortOrder OrderBy { get; set; }

    public int TotalItems { get; set; }
}