using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Extensions;
using Microsoft.IdentityModel.Tokens;
using SendGrid.Helpers.Errors.Model;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using TFAuto.DAL.Entities.Article;
using TFAuto.Domain.Services.ArticlePage.DTO.Request;
using TFAuto.Domain.Services.ArticlePage.DTO.Response;
using TFAuto.Domain.Services.Authentication.Constants;
using TFAuto.Domain.Services.Blob;
using TFAuto.TFAuto.DAL.Entities;

namespace TFAuto.Domain.Services.ArticlePage;
public class ArticleService : IArticleService
{
    private readonly IRepository<Article> _repositoryArticle;
    private readonly IRepository<User> _repositoryUser;
    private readonly IRepository<Tag> _repositoryTag;
    private readonly IBlobService _imageService;
    private readonly IHttpContextAccessor _contextAccessor;
    private readonly IMapper _mapper;

    public ArticleService(IRepository<Article> repositoryArticle, IRepository<User> repositoryUser, IRepository<Tag> repositoryTag, IBlobService imageService, IHttpContextAccessor contextAccessor, IMapper mapper)
    {
        _repositoryArticle = repositoryArticle;
        _repositoryUser = repositoryUser;
        _repositoryTag = repositoryTag;
        _imageService = imageService;
        _contextAccessor = contextAccessor;
        _mapper = mapper;
    }

    public async ValueTask<CreateArticleResponse> CreateArticleAsync(CreateArticleRequest articleRequest)
    {
        var articleAuthorId = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == CustomClaimsType.USER_ID)?.Value;
        var articleAuthorName = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == CustomClaimsType.USER_NAME)?.Value;

        if (articleAuthorId == null || articleAuthorName == null)
            throw new ValidationException(ErrorMessages.ARTICLE_USER_NOT_FOUND);

        Article articleEntityFromRequest = _mapper.Map<Article>(articleRequest);

        var articleAuthorEntity = await _repositoryUser.GetAsync(c => c.Id == articleAuthorId).FirstOrDefaultAsync();

        if (articleAuthorEntity == null)
            throw new ValidationException(ErrorMessages.ARTICLE_USER_NOT_FOUND);

        articleAuthorEntity.ArticleIds.Add(articleEntityFromRequest.Id);
        await _repositoryUser.UpdateAsync(articleAuthorEntity);

        List<Tag> tagsForArticleEntityList = await AllocateTags(articleRequest.Tags, articleEntityFromRequest);

        var imageResponse = await _imageService.UploadAsync(articleRequest.Image);
        articleEntityFromRequest.UserId = articleAuthorId;
        articleEntityFromRequest.UserName = articleAuthorName;
        articleEntityFromRequest.LastUserWhoUpdated = articleAuthorName;
        articleEntityFromRequest.ImageFileName = imageResponse.FileName;

        var dataArticleEntity = await _repositoryArticle.CreateAsync(articleEntityFromRequest);

        CreateArticleResponse articleResponse = _mapper.Map<CreateArticleResponse>(dataArticleEntity);
        articleResponse.Image = imageResponse;

        foreach (Tag tag in tagsForArticleEntityList)
        {
            TagResponse tagResponse = _mapper.Map<TagResponse>(tag);
            articleResponse.Tags.Add(tagResponse);
        }

        return articleResponse;
    }

    public async ValueTask<UpdateArticleResponse> UpdateArticleAsync(Guid articleId, UpdateArticleRequest articleRequest)
    {
        var lastArticleAuthorName = _contextAccessor.HttpContext.User.Claims.FirstOrDefault(c => c.Type == CustomClaimsType.USER_NAME)?.Value;

        if (lastArticleAuthorName == null)
            throw new ValidationException(ErrorMessages.ARTICLE_USER_WHO_UPDATED_NOT_FOUND);

        Article articleEntityFromRequest = _mapper.Map<Article>(articleRequest);

        var existingArticleEntity = await _repositoryArticle.GetAsync(c => c.Id == articleId.ToString()).FirstOrDefaultAsync();

        if (existingArticleEntity == null)
            throw new NotFoundException(ErrorMessages.ARTICLE_NOT_FOUND);

        foreach (var existingArticleEntityTagId in existingArticleEntity.TagIds)
        {
            var tagUpdatedByArticle = await _repositoryTag.GetAsync(c => c.Id == existingArticleEntityTagId).FirstOrDefaultAsync();
            tagUpdatedByArticle.ArticleIds.Remove(existingArticleEntity.Id);
            await _repositoryTag.UpdateAsync(tagUpdatedByArticle);
        }

        existingArticleEntity.TagIds.Clear();

        List<Tag> newArticleEntityTagsList = await AllocateTags(articleRequest.Tags, existingArticleEntity);

        var imageResponse = await _imageService.UpdateAsync(existingArticleEntity.ImageFileName, articleRequest.Image);

        existingArticleEntity.Name = articleEntityFromRequest.Name;
        existingArticleEntity.Description = articleEntityFromRequest.Description;
        existingArticleEntity.Text = articleEntityFromRequest.Text;
        existingArticleEntity.LastUserWhoUpdated = lastArticleAuthorName;
        existingArticleEntity.ImageFileName = imageResponse.FileName;

        var dataArticle = await _repositoryArticle.UpdateAsync(existingArticleEntity);

        UpdateArticleResponse articleResponse = _mapper.Map<UpdateArticleResponse>(dataArticle);

        foreach (Tag tag in newArticleEntityTagsList)
        {
            TagResponse tagResponse = _mapper.Map<TagResponse>(tag);
            articleResponse.Tags.Add(tagResponse);
        }

        articleResponse.Image = imageResponse;

        return articleResponse;
    }

    public async ValueTask<GetArticleResponse> GetArticleAsync(Guid articleId)
    {
        var article = await _repositoryArticle.GetAsync(c => c.Id == articleId.ToString()).FirstOrDefaultAsync();

        if (article == null)
            throw new NotFoundException(ErrorMessages.ARTICLE_NOT_FOUND);

        var articleResponse = await ConvertGetArticleResponse(article);

        return articleResponse;
    }

    public async ValueTask<GetAllArticlesResponse> GetAllArticlesAsync(BasePaginationRequest paginationRquest)
    {
        string QUERY_ARTICLES = $"SELECT * FROM c WHERE c.type = \"{nameof(Article)}\"";

        string QUERY_SORTING_ARTICLES(string sortByParameter, string orderByParameter = "")
        {
            QUERY_ARTICLES = $"SELECT * FROM c WHERE c.type = \"{nameof(Article)}\" ORDER BY c.{sortByParameter} {orderByParameter}";
            return QUERY_ARTICLES;
        }

        if (paginationRquest.Skip < 0 || paginationRquest.Take < 1)
            throw new Exception(ErrorMessages.PAGE_NOT_EXISTS);

        if (!paginationRquest.SortBy.ToString().IsNullOrEmpty())
        {
            switch (paginationRquest.SortBy.ToString())
            {
                case nameof(SortOrder.ByTheme):
                    QUERY_ARTICLES = QUERY_SORTING_ARTICLES(nameof(Article.Name).ToLower());
                    break;

                case nameof(SortOrder.Ascending):
                    QUERY_ARTICLES = QUERY_SORTING_ARTICLES(nameof(Article.LastUpdatedTimeUtc));
                    break;

                case nameof(SortOrder.Descending):
                    QUERY_ARTICLES = QUERY_SORTING_ARTICLES(nameof(Article.LastUpdatedTimeUtc), "DESC");
                    break;
            }
        }

        var articleList = await _repositoryArticle.GetByQueryAsync(QUERY_ARTICLES);

        if (articleList == null)
            throw new NotFoundException(ErrorMessages.ARTICLE_NOT_FOUND);

        var totalItems = articleList.Count();

        if (totalItems <= paginationRquest.Skip)
            throw new NotFoundException(ErrorMessages.ARTICLE_NOT_FOUND);

        if ((totalItems - paginationRquest.Skip) < paginationRquest.Take)
            paginationRquest.Take = (totalItems - paginationRquest.Skip);

        var allArticlesResponse = new GetAllArticlesResponse()
        {
            TotalItems = totalItems,
            Skip = paginationRquest.Skip,
            Take = paginationRquest.Take,
            OrderBy = paginationRquest.SortBy,
            Articles = articleList
            .Skip(paginationRquest.Skip)
            .Take(paginationRquest.Take)
            .Select(async article => await ConvertGetArticleResponse(article))
            .Select(task => task.Result).ToList()
        };

        return allArticlesResponse;
    }

    private async ValueTask<List<Tag>> AllocateTags(string tagString, Article articleEntity)
    {
        const int TAGS_MAX_QUANTITY = 5;
        const string TAGS_PATTERN = @"#[A-Za-z0-9]+";

        List<Tag> tagsForArticleEntityList = new();

        if (tagString.IsNullOrEmpty())
        {
            return tagsForArticleEntityList;
        }

        MatchCollection selectedTagsFromString = Regex.Matches(tagString.ToLower(), TAGS_PATTERN);

        if (selectedTagsFromString.Count > TAGS_MAX_QUANTITY)
            throw new ValidationException(ErrorMessages.ARTICLE_MAX_TAGS_QUANTITY);

        foreach (Match selectedTagFromString in selectedTagsFromString)
        {
            string selectedTagNameFromString = selectedTagFromString.Value;

            var existingEntityTag = await _repositoryTag.GetAsync(c => c.Name == selectedTagNameFromString).FirstOrDefaultAsync();

            if (existingEntityTag == null)
            {
                var newTag = new Tag
                {
                    Name = selectedTagNameFromString,
                    ArticleIds = new List<string> { articleEntity.Id }
                };
                await _repositoryTag.CreateAsync(newTag);

                articleEntity.TagIds.Add(newTag.Id);
                tagsForArticleEntityList.Add(newTag);
            }
            else
            {
                articleEntity.TagIds.Add(existingEntityTag.Id);
                tagsForArticleEntityList.Add(existingEntityTag);

                existingEntityTag.ArticleIds.Add(articleEntity.Id);
                await _repositoryTag.UpdateAsync(existingEntityTag);
            }
        }

        return tagsForArticleEntityList;
    }

    private async ValueTask<GetArticleResponse> ConvertGetArticleResponse(Article article)
    {
        string queryTagsByArticleId = $"SELECT * FROM c WHERE c.type = \"{nameof(Tag)}\" AND ARRAY_CONTAINS(c.articleIds, '{article.Id}')";
        var tagsList = await _repositoryTag.GetByQueryAsync(queryTagsByArticleId);
        var imageResponse = await _imageService.GetAsync(article.ImageFileName);

        GetArticleResponse articleResponse = _mapper.Map<GetArticleResponse>(article);
        articleResponse.Image = imageResponse;
        articleResponse.Tags = tagsList.Select(tag => _mapper.Map<TagResponse>(tag)).ToList();

        return articleResponse;
    }
}
