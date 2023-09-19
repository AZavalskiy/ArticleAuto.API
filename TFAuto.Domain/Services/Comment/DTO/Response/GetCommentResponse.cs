namespace TFAuto.Domain.Services.CommentService.DTO
{
    public class GetCommentResponse
    {
        public string Id { get; set; }

        public string Content { get; set; }

        public int LikesCount { get; set; }

        public string AuthorId { get; set; }

        public string ArticleId { get; set; }

    }
}