using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TFAuto.Domain.Services.ArticlePage.DTO.Request;

public class CreateArticleRequest
{
    [Required]
    public IFormFile Image { get; set; }

    [Required]
    [DefaultValue("Name of the article")]
    public string Name { get; set; }

    [Required]
    [DefaultValue("Description of the article")]
    public string Description { get; set; }

    [Required]
    [DefaultValue("Text of the article")]
    public string Text { get; set; }

    [DefaultValue("#tag1 #tag2 #tag3")]
    public string Tags { get; set; }
}
