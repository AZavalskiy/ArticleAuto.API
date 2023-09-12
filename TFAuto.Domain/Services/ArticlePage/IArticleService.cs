using TFAuto.Domain.Services.ArticlePage.DTO.Request;
using TFAuto.Domain.Services.ArticlePage.DTO.Response;

namespace TFAuto.Domain.Services.ArticlePage;

public interface IArticleService
{
    ValueTask<CreateArticleResponse> CreateArticleAsync(CreateArticleRequest articleRequest);

    ValueTask<UpdateArticleResponse> UpdateArticleAsync(Guid articleId, UpdateArticleRequest articleRequest);

    ValueTask<GetArticleResponse> GetArticleAsync(Guid articleId);

    ValueTask<GetAllArticlesResponse> GetAllArticlesAsync(int page);
}
