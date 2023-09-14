namespace TFAuto.Domain.Services.CommentService.DTO
{
    public class PagedCommentResponse
    {
        public IEnumerable<GetCommentResponse> Comments { get; set; }

        public int CurrentPage { get; set; }

        public int? Pages { get; set; }

        public int PageSize { get; set; }

        public int CommentsCount { get; set; }

    }
}