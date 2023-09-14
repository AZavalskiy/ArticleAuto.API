using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TFAuto.Domain.Services.CommentService.DTO
{
    public class GetCommentPaginationRequest
    {
        [Required]
        [DefaultValue(0)]
        public int Skip { get; set; }

        [Required]
        [DefaultValue(25)]
        public int Take { get; set; }
    }
}