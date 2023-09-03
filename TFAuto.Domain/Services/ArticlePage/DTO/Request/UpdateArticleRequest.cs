using Microsoft.AspNetCore.Http;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace TFAuto.Domain.Services.ArticlePage.DTO.Request;

public class UpdateArticleRequest
{
    public IFormFile Image { get; set; }

    [Required]
    [DefaultValue("Jokes about cars")]
    public string Name { get; set; }

    [Required]
    [DefaultValue("Exploring the Humorous World of Car Jokes")]
    public string Description { get; set; }

    [Required]
    [DefaultValue("The End.")]
    public string Text { get; set; }

    [DefaultValue("#ilikecars #carsarecool #bmw #ladasedan")]
    public string Tags { get; set; }
}
