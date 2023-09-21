using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Extensions;
using Microsoft.IdentityModel.Tokens;
using SendGrid.Helpers.Errors.Model;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.RegularExpressions;
using TFAuto.DAL.Entities.Article;
using TFAuto.Domain.ExtensionMethods;
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

    public async ValueTask<GetAllArticlesResponse> GetAllArticlesAsync(GetArticlesPaginationRequest paginationRequest)
    {
        const int PAGINATION_SKIP_MIN_LIMIT = 0;
        const int PAGINATION_TAKE_MIN_LIMIT = 1;

        if (paginationRequest.Skip < PAGINATION_SKIP_MIN_LIMIT || paginationRequest.Take < PAGINATION_TAKE_MIN_LIMIT)
            throw new Exception(ErrorMessages.PAGE_NOT_EXISTS);

        string queryArticles = await BuildQuery(paginationRequest);
        var articleList = await _repositoryArticle.GetByQueryAsync(queryArticles);

        if (articleList == null)
            throw new NotFoundException(ErrorMessages.ARTICLE_NOT_FOUND);

        var totalItems = articleList.Count();

        if (totalItems <= paginationRequest.Skip)
            throw new NotFoundException(ErrorMessages.ARTICLE_NOT_FOUND);

        if ((totalItems - paginationRequest.Skip) < paginationRequest.Take)
            paginationRequest.Take = (totalItems - paginationRequest.Skip);

        var articlesResponse = articleList
            .Skip(paginationRequest.Skip)
            .Take(paginationRequest.Take)
            .Select(async article => await ConvertGetArticleResponse(article))
            .WhenAll().ToList();

        var allArticlesResponse = new GetAllArticlesResponse()
        {
            TotalItems = totalItems,
            Skip = paginationRequest.Skip,
            Take = paginationRequest.Take,
            OrderBy = paginationRequest.SortBy,
            Articles = articlesResponse
        };

        return allArticlesResponse;
    }

    private async ValueTask<List<Tag>> AllocateTags(List<string> tagsList, Article articleEntity)
    {
        const int TAGS_MAX_QUANTITY = 5;
        const string TAGS_PATTERN = @"#[A-Za-z0-9]+";

        if (tagsList.Count > TAGS_MAX_QUANTITY)
            throw new ValidationException(ErrorMessages.ARTICLE_MAX_TAGS_QUANTITY);

        List<Tag> tagsForArticleEntityList = new();

        if (tagsList.IsNullOrEmpty())
        {
            return tagsForArticleEntityList;
        }

        List<string> matchingTags = new();

        foreach (var tag in tagsList)
        {
            MatchCollection matchedTagsFromList = Regex.Matches(tag, TAGS_PATTERN);

            foreach (Match match in matchedTagsFromList)
            {
                matchingTags.Add(match.Value);
            }
        }

        foreach (var tag in matchingTags)
        {
            var existingEntityTag = await _repositoryTag.GetAsync(c => c.Name == tag).FirstOrDefaultAsync();

            if (existingEntityTag == null)
            {
                var newTag = new Tag
                {
                    Name = tag,
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

    private async ValueTask<string> BuildQuery(GetArticlesPaginationRequest paginationRequest)
    {
        List<Tag> tagsList = new();

        const string baseQuery = $"SELECT * FROM c WHERE c.type = \"{nameof(Article)}\" ";
        StringBuilder queryBuilder = new(baseQuery);

        if (!paginationRequest.Author.IsNullOrEmpty())
        {
            queryBuilder.Append($"AND CONTAINS(LOWER(c.{nameof(Article.UserName).FirstLetterToLower()}), LOWER(\"{paginationRequest.Author}\")) ");
        }

        if (!paginationRequest.Tags.IsNullOrEmpty())
        {
            foreach (var tag in paginationRequest.Tags)
            {
                if (tag == null)
                    throw new NotFoundException(ErrorMessages.ARTICLE_NOT_FOUND);

                var tagEntity = await _repositoryTag.GetAsync(c => c.Name.ToLower() == tag.ToLower()).FirstOrDefaultAsync();

                if (tagEntity == null)
                    throw new NotFoundException(ErrorMessages.ARTICLE_NOT_FOUND);

                queryBuilder.Append($"AND ARRAY_CONTAINS(c.{nameof(Article.TagIds).FirstLetterToLower()}, \"{tagEntity.Id}\") ");
            }
        }

        if (!paginationRequest.Text.IsNullOrEmpty())
        {
            queryBuilder.Append(
                $"AND CONTAINS(LOWER(c.{nameof(Article.Name).FirstLetterToLower()}), LOWER(\"{paginationRequest.Text}\")) " +
                $"OR CONTAINS(LOWER(c.{nameof(Article.Text).FirstLetterToLower()}), LOWER(\"{paginationRequest.Text}\")) ");
        }

        queryBuilder.Append(" ORDER BY c.");

        if (paginationRequest.SortBy.ToString() == nameof(SortOrder.Ascending))
        {
            queryBuilder.Append(nameof(Article.LastUpdatedTimeUtc));
        }
        else if (paginationRequest.SortBy.ToString() == nameof(SortOrder.ByTheme))
        {
            queryBuilder.Append(nameof(Article.LastUpdatedTimeUtc));
        }
        else
        {
            queryBuilder.Append(nameof(Article.LastUpdatedTimeUtc));
            queryBuilder.Append(" DESC");
        }

        return queryBuilder.ToString();
    }

    private async ValueTask<GetArticleResponse> ConvertGetArticleResponse(Article article)
    {
        string queryTagsByArticleId = $"SELECT * FROM c WHERE c.type = \"{nameof(Tag)}\" AND ARRAY_CONTAINS(c.{nameof(Tag.ArticleIds).FirstLetterToLower()}, '{article.Id}')";
        var tagsList = await _repositoryTag.GetByQueryAsync(queryTagsByArticleId);
        var imageResponse = await _imageService.GetAsync(article.ImageFileName);

        GetArticleResponse articleResponse = _mapper.Map<GetArticleResponse>(article);
        articleResponse.Image = imageResponse;
        articleResponse.Tags = tagsList.Select(tag => _mapper.Map<TagResponse>(tag)).ToList();

        return articleResponse;
    }
}