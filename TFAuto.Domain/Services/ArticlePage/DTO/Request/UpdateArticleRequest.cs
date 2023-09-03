using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TFAuto.Domain.Services.ArticlePage.DTO.Request;

public class UpdateArticleRequest
{
    public IFormFile Image { get; set; }

    [Required]
    [DefaultValue("New name of the article")]
    public string Name { get; set; }

    [Required]
    [DefaultValue("New description of the article")]
    public string Description { get; set; }

    [Required]
    [DefaultValue("New text of the article")]
    public string Text { get; set; }

    [DefaultValue("#tag4 #tag5 #tag6")]
    public string Tags { get; set; }
}
