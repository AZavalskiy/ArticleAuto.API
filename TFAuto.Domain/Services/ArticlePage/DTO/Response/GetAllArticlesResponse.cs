namespace TFAuto.Domain.Services.ArticlePage.DTO.Response;

public class GetAllArticlesResponse
{
    public int Pages { get; set; }

    public int CurrentPage { get; set; }

    public List<GetArticleResponse> Articles { get; set; } = new();
}
