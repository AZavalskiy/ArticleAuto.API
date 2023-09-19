using TFAuto.Domain.Services.CommentService.Pagination;

namespace TFAuto.Domain.Services.CommentService.DTO
{
    public class GetAllCommentsResponse : BasePaginationComment
    {
        public List<GetCommentResponse> Comments { get; set; } = new();

        public SortCommentOrder OrderBy { get; set; }
    }
}