using System.ComponentModel;

namespace TFAuto.Domain.Services.CommentService.Pagination;

public class BasePaginationCommentsRequest
{
    [DefaultValue(0)]
    public int Skip { get; set; }

    [DefaultValue(25)]
    public int Take { get; set; }

    [DefaultValue(nameof(SortCommentOrder.ByDate))]
    public SortCommentOrder SortBy { get; set; }
}