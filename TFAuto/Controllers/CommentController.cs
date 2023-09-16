using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using TFAuto.DAL.Constant;
using TFAuto.Domain.Services.CommentService;
using TFAuto.Domain.Services.CommentService.DTO;
using TFAuto.Domain.Services.LikeService;

namespace TFAuto.WebApp.Controllers
{
    [ApiController]
    [Produces("application/json")]
    [Route("comments")]
    [Authorize]

    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly ILikeService _likeService;

        public CommentController(ICommentService commentService, ILikeService likeService)
        {
            _commentService = commentService;
            _likeService = likeService;
        }

        [HttpPost]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(CreateCommentResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async ValueTask<ActionResult<CreateCommentResponse>> AddCommentAsync([FromQuery] CreateCommentRequest commentCreate)
        {
            var createdComment = await _commentService.AddCommentAsync(commentCreate);
            return Ok(createdComment);
        }

        [HttpPut("{id:Guid}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(UpdateCommentResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async ValueTask<ActionResult<UpdateCommentResponse>> UpdateCommentAsync([Required] Guid id, [FromQuery] UpdateCommentRequest commentUpdate)
        {
            var updatedComment = await _commentService.UpdateCommentAsync(id, commentUpdate);
            return Ok(updatedComment);
        }

        [HttpDelete("{id:Guid}")]
        [Authorize(Policy = PermissionId.DELETE_COMMENT)]
        [SwaggerResponse(StatusCodes.Status204NoContent)]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async ValueTask<IActionResult> DeleteCommentAsync([Required] Guid id, [FromQuery] DeleteCommentRequest commentDelete)
        {
            await _commentService.DeleteCommentAsync(id, commentDelete);
            return NoContent();
        }

        [HttpGet]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(PagedCommentResponse))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async ValueTask<ActionResult<PagedCommentResponse>> GetArticleCommentsByPageAsync([Required] Guid articalId, [FromQuery] GetCommentRequest getComment)
        {
            var comments = await _commentService.GetArticleCommentsByPageAsync(articalId, getComment);
            return Ok(comments);
        }

        [HttpPost("like/{commentId}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(bool))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async ValueTask<ActionResult<bool>> GiveLikeCommentAsync(Guid commentId, [Required] Guid userId)
        {
            var like = await _likeService.GiveLikeCommentAsync(commentId, userId);
            return Ok(like);
        }

        [HttpDelete("like/{commentId}")]
        [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(bool))]
        [SwaggerResponse(StatusCodes.Status400BadRequest)]
        [SwaggerResponse(StatusCodes.Status500InternalServerError)]
        public async ValueTask<ActionResult<bool>> RemoveLikeCommentAsync(Guid commentId, [Required] Guid userId)
        {
            var unlike = await _likeService.RemoveLikeCommentAsync(commentId, userId);
            return Ok(unlike);
        }

    }
}