using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.ComponentModel.DataAnnotations;
using TFAuto.DAL.Constant;
using TFAuto.Domain.Services.ArticlePage;
using TFAuto.Domain.Services.ArticlePage.DTO.Request;
using TFAuto.Domain.Services.ArticlePage.DTO.Response;

namespace TFAuto.WebApp.Controllers;

[Route("articles")]
[ApiController]
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

    [HttpPut("{id:Guid}")]
    [Authorize(Policy = PermissionId.MANAGE_ARTICLES)]
    [SwaggerOperation(
     Summary = "Update an article",
     Description = "Update an article by an user with a superadmin role")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(UpdateArticleResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async ValueTask<ActionResult<UpdateArticleResponse>> UpdateArticleAsync([FromRoute] Guid id, [FromForm] UpdateArticleRequest articleRequest)
    {
        var updatedArticle = await _articleService.UpdateArticleAsync(id, articleRequest);
        return Ok(updatedArticle);
    }

    [HttpGet("{id:Guid}")]
    [Authorize]
    [SwaggerOperation(
     Summary = "Retrive an article by id",
     Description = "Retrives an article with tags and image")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(GetArticleResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async ValueTask<ActionResult<GetArticleResponse>> GetArticleAsync([FromRoute] Guid id)
    {
        var retrievedArticle = await _articleService.GetArticleAsync(id);
        return Ok(retrievedArticle);
    }

    [HttpGet]
    [Authorize]
    [SwaggerOperation(
     Summary = "Retrive all articles with pagination",
     Description = "Retrives all articles by pages, 3 articles per page")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(GetAllArticlesResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async ValueTask<ActionResult<GetAllArticlesResponse>> GetAllArticlesAsync([Required][FromQuery] int page)
    {
        var retrievedArticles = await _articleService.GetAllArticlesAsync(page);
        return Ok(retrievedArticles);
    }
}
