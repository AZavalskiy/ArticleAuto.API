using System.ComponentModel;

namespace TFAuto.Domain.Services;

public class GetArticlesPaginationRequest
{
    public string Text { get; set; }

    public string Author { get; set; }

    public List<string> Tags { get; set; } = new();

    [DefaultValue(0)]
    public int Skip { get; set; }

    [DefaultValue(25)]
    public int Take { get; set; }

    public SortOrder SortBy { get; set; } = SortOrder.Descending;
}
