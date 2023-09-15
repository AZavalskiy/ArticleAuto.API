using System.ComponentModel;

namespace TFAuto.Domain.Services;

public class BasePaginationRequest
{
    [DefaultValue(0)]
    public int Skip { get; set; }

    [DefaultValue(25)]
    public int Take { get; set; }

    [DefaultValue(nameof(SortOrder.Ascending))]
    public SortOrder SortBy { get; set; }
}
