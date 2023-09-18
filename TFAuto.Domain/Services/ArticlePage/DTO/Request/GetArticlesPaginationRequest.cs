using System.ComponentModel;

namespace TFAuto.Domain.Services;

public class GetArticlesPaginationRequest
{
    [DefaultValue(0)]
    public int Skip { get; set; }

    [DefaultValue(25)]
    public int Take { get; set; }

    public SortOrder SortBy { get; set; } = SortOrder.Descending;
}
