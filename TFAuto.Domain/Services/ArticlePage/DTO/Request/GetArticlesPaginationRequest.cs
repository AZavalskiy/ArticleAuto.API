namespace TFAuto.Domain.Services;

public class GetArticlesPaginationRequest : BasePaginationRequest
{
    public string Text { get; set; }

    public string Author { get; set; }

    public List<string> Tags { get; set; } = new();

    public SortOrder SortBy { get; set; } = SortOrder.Descending;
}
