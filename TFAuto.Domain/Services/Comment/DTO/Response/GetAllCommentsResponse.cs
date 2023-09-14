namespace TFAuto.Domain.Services.CommentService.DTO
{
    public class GetAllCommentsResponse : BasePagination
    {
        public IEnumerable<GetCommentResponse> Comments { get; set; }
    }
}