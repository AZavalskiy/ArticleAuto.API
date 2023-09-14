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
     Description = "Creates an article by an author")]
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
     Description = "Updates an article created by author")]
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
     Summary = "Retrieve an article by id",
     Description = "Retrieves an article with tags and an image")]
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
     Summary = "Retrieve articles with pagination",
     Description = "Retrieves articles by skip and take parameters and sorting")]
    [SwaggerResponse(StatusCodes.Status200OK, "Success", typeof(GetAllArticlesResponse))]
    [SwaggerResponse(StatusCodes.Status400BadRequest)]
    [SwaggerResponse(StatusCodes.Status404NotFound)]
    [SwaggerResponse(StatusCodes.Status500InternalServerError)]
    public async ValueTask<ActionResult<GetAllArticlesResponse>> GetAllArticlesAsync([Required][FromQuery] int skip, [Required] int take, [Required] SortOrder sortBy)
    {
        var retrievedArticles = await _articleService.GetAllArticlesAsync(skip, take, sortBy);
        return Ok(retrievedArticles);
    }
}
