namespace TFAuto.Domain.Services.ArticlePage.DTO.Response;

public class GetTopAuthorsResponse : BasePagination
{
    public List<GetAuthorResponse> Authors { get; set; } = new();
}
