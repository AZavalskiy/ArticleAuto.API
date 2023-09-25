namespace TFAuto.Domain.Services.ArticlePage.DTO.Response;

public class GetAllArticlesResponse : BasePagination
{
    public List<GetArticleResponse> Articles { get; set; } = new();

    public int TotalItems { get; set; }

    public SortOrder OrderBy { get; set; }
}