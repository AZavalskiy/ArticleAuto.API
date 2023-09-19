using TFAuto.Domain.Services.CommentService.DTO;
using TFAuto.Domain.Services.CommentService.Pagination;

namespace TFAuto.Domain.Services.CommentService
{
    public interface ICommentService
    {
        ValueTask<CreateCommentResponse> AddCommentAsync(CreateCommentRequest commentCreate);

        ValueTask<UpdateCommentResponse> UpdateCommentAsync(Guid id, UpdateCommentRequest commentUpdate);

        ValueTask DeleteCommentAsync(Guid id, DeleteCommentRequest commentDelete);

        ValueTask<GetAllCommentsResponse> GetAllCommentsAsync(BasePaginationCommentsRequest paginationRequest);
    }
}
