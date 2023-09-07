using TFAuto.Domain.Services.ArticlePage.DTO.Request;
using TFAuto.Domain.Services.ArticlePage.DTO.Response;

namespace TFAuto.Domain.Services.ArticlePage;

public interface IArticleService
{
    public ValueTask<CreateArticleResponse> CreateArticleAsync(CreateArticleRequest articleRequest);

    public ValueTask<UpdateArticleResponse> UpdateArticleAsync(Guid articleId, UpdateArticleRequest articleRequest);
}
