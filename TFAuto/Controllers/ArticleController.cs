using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TFAuto.Domain.Services.ArticlePage;
using TFAuto.Domain.Services.ArticlePage.DTO.Request;
using TFAuto.Domain.Services.ArticlePage.DTO.Response;

namespace TFAuto.WebApp.Controllers;

[Route("article")]
[ApiController]
[Authorize]
public class ArticleController : ControllerBase
{
    private readonly IArticleService _articleService;

    public ArticleController(IArticleService articleService)
    {
        _articleService = articleService;
    }

    [HttpPost]
    [SwaggerOperation(
   Summary = "Create an article",
   Description = "Creates an article by an user with an author or superadmin role")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(CreateArticleResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async ValueTask<ActionResult<CreateArticleResponse>> CreateArticleAsyncs([FromForm] CreateArticleRequest articleRequest)
    {
        var createdArticle = await _articleService.CreateArticleAsync(articleRequest);
        return Ok(createdArticle);
    }

    [HttpPut]
    [SwaggerOperation(
    Summary = "Update an article",
    Description = "Update an article by an user with a superadmin role")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(CreateArticleResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async ValueTask<ActionResult<UpdateArticleResponse>> UpdateArticleAsync([FromQuery] string articleId, [FromForm] UpdateArticleRequest articleRequest)
    {
        var updatedArticle = await _articleService.UpdateArticleAsync(articleId, articleRequest);
        return Ok(updatedArticle);
    }
}
