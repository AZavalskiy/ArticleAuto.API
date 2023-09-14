namespace TFAuto.Domain.Services.ArticlePage.DTO.Response;

public class GetAllArticlesResponse : PagingBase
{
    public List<GetArticleResponse> Articles { get; set; } = new();
}

public enum SortOrder
{
    Ascending,

    Descending,

    ByTheme,
}