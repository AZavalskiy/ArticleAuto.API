using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using TFAuto.DAL.Constant;
using TFAuto.Domain.Services.ArticlePage;
using TFAuto.Domain.Services.ArticlePage.DTO.Request;
using TFAuto.Domain.Services.ArticlePage.DTO.Response;

namespace TFAuto.WebApp.Controllers;

[Route("users/articles")]
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
    [Authorize(Policy = PermissionId.EDIT_ARTICLES)]
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

    [HttpPut("{id}")]
    [Authorize(Policy = PermissionId.MANAGE_ARTICLES)]
    [SwaggerOperation(
    Summary = "Update an article",
    Description = "Update an article by an user with a superadmin role")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(CreateArticleResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async ValueTask<ActionResult<UpdateArticleResponse>> UpdateArticleAsync([FromRoute] string id, [FromForm] UpdateArticleRequest articleRequest)
    {
        var updatedArticle = await _articleService.UpdateArticleAsync(id, articleRequest);
        return Ok(updatedArticle);
    }
}
