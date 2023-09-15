namespace TFAuto.Domain.Services.ArticlePage.DTO.Response;

public class GetAllArticlesResponse : BasePagination
{
    public List<GetArticleResponse> Articles { get; set; } = new();

    public SortOrder OrderBy { get; set; }
}