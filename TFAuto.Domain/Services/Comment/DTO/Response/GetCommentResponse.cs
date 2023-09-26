namespace TFAuto.Domain.Services.CommentService.DTO
{
    public class GetCommentResponse
    {
        public string Id { get; set; }

        public string Content { get; set; }

        public int LikesCount { get; set; }

        public string AuthorId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public string ArticleId { get; set; }

    }
}