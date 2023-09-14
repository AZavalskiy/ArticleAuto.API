namespace TFAuto.Domain.Services.CommentService.Pagination;

public class BasePaginationComment
{
    public int TotalItems { get; set; }

    public int Take { get; set; }

    public int Skip { get; set; }
}