using AutoMapper;
using Microsoft.Azure.CosmosRepository;
using Microsoft.Azure.CosmosRepository.Extensions;
using SendGrid.Helpers.Errors.Model;
using System.ComponentModel.DataAnnotations;
using TFAuto.DAL.Constant;
using TFAuto.DAL.Entities;
using TFAuto.DAL.Entities.Article;
using TFAuto.Domain.Services.CommentService.DTO;
using TFAuto.Domain.Services.LikeService;

namespace TFAuto.Domain.Services.CommentService
{
    public class CommentService : ICommentService
    {
        private readonly IRepository<Comment> _repositoryComment;
        private readonly IRepository<Article> _repositoryArticle;
        private readonly IRepository<TFAuto.DAL.Entities.User> _repositoryUser;
        private readonly IRepository<Role> _repositoryRole;
        private readonly IMapper _mapper;
        private readonly ILikeService _likeService;

        public CommentService(
            IRepository<Comment> repositoryComment,
            IRepository<Article> repositoryArticle,
            IRepository<TFAuto.DAL.Entities.User> repositoryUser,
            IRepository<Role> repositoryRole,
            IMapper mapper,
            ILikeService likeService)
        {
            _repositoryComment = repositoryComment;
            _repositoryArticle = repositoryArticle;
            _repositoryUser = repositoryUser;
            _repositoryRole = repositoryRole;
            _mapper = mapper;
            _likeService = likeService;
        }

        public async ValueTask<CreateCommentResponse> AddCommentAsync(CreateCommentRequest commentCreate)
        {
            var article = await _repositoryArticle.GetAsync(t => t.Id == commentCreate.ArticleId.ToString()).FirstOrDefaultAsync();

            if (article == null)
                throw new NotFoundException(ErrorMessages.ARTICLE_NOT_FOUND);

            var user = await _repositoryUser.GetAsync(t => t.Id == commentCreate.AuthorId.ToString()).FirstOrDefaultAsync();

            if (user == null)
                throw new NotFoundException(ErrorMessages.USER_NOT_FOUND);

            var commentMapped = _mapper.Map<Comment>(commentCreate);
            commentMapped.ArticleId = article.Id;
            var createdComment = await _repositoryComment.CreateAsync(commentMapped);

            var createdCommentResponce = _mapper.Map<CreateCommentResponse>(createdComment);

            return createdCommentResponce;
        }

        public async ValueTask<UpdateCommentResponse> UpdateCommentAsync(Guid id, UpdateCommentRequest commentUpdate)
        {
            var comment = await _repositoryComment.GetAsync(t => t.Id == id.ToString()).FirstOrDefaultAsync();

            if (comment == null)
                throw new NotFoundException(ErrorMessages.COMMENT_NOT_FOUND);

            var user = await _repositoryUser.GetAsync(t => t.Id == commentUpdate.UserId.ToString()).FirstOrDefaultAsync();

            if (user == null)
                throw new NotFoundException(ErrorMessages.USER_NOT_FOUND);

            var article = await _repositoryArticle.GetAsync(t => t.Id == id.ToString()).FirstOrDefaultAsync();

            if (user.Id != comment.AuthorId)
                throw new ValidationException(ErrorMessages.USER_IS_NOT_COMMENT_AUTHOR);

            var commentMapped = _mapper.Map<Comment>(commentUpdate);
            var updatedComment = await _repositoryComment.CreateAsync(commentMapped);

            updatedComment.ArticleId = article.Id;
            var updatedCommentResponce = _mapper.Map<UpdateCommentResponse>(updatedComment);

            return updatedCommentResponce;
        }

        public async ValueTask DeleteCommentAsync(Guid id, DeleteCommentRequest commentDelete)
        {
            var comment = await _repositoryComment.GetAsync(t => t.Id == id.ToString()).FirstOrDefaultAsync();

            if (comment == null)
                throw new NotFoundException(ErrorMessages.COMMENT_NOT_FOUND);

            var user = await _repositoryUser.GetAsync(t => t.Id == commentDelete.UserId.ToString()).FirstOrDefaultAsync();

            if (user == null)
                throw new NotFoundException(ErrorMessages.USER_NOT_FOUND);

            var role = await _repositoryRole.GetAsync(user.RoleId, nameof(Role));

            if (role.RoleName == RoleNames.SUPER_ADMIN)
            {
                await _repositoryComment.DeleteAsync(comment);
                await _likeService.RemoveLikesByCommentAsync(comment.Id);
            }
            else
            {
                var article = await _repositoryArticle.GetAsync(t => t.Id == comment.ArticleId.ToString()).FirstOrDefaultAsync();

                if (user.Id != article.UserId)
                    throw new ValidationException(ErrorMessages.USER_IS_NOT_ARTICLE_AUTHOR);

                await _repositoryComment.DeleteAsync(comment);
                await _likeService.RemoveLikesByCommentAsync(comment.Id);
            }
        }

        public async ValueTask<PagedCommentResponse> GetArticleCommentsByPageAsync(Guid articleId, GetCommentRequest getComment)
        {
            var article = await _repositoryArticle.GetAsync(t => t.Id == articleId.ToString()).FirstOrDefaultAsync();

            if (article == null)
                throw new NotFoundException(ErrorMessages.ARTICLE_NOT_FOUND);

            var commentList = await _repositoryComment.GetAsync(c => c.ArticleId == articleId.ToString());

            if (commentList is null)
                throw new NotFoundException(ErrorMessages.COMMENTS_NOT_FOUND);

            var pageResults = getComment.PageSize;
            string continuationToken = null;

            if (getComment.CurrentPage > 1)
            {
                var skip = (getComment.CurrentPage - 1) * pageResults;
                var previousPage = await _repositoryComment.PageAsync(
                    c => c.ArticleId == articleId.ToString(),
                    pageResults,
                    continuationToken: null);

                if (previousPage != null && skip < previousPage.Total)
                {
                    continuationToken = previousPage.Continuation;
                }
            }

            var pageResult = await _repositoryComment.PageAsync(
                c => c.ArticleId == articleId.ToString(),
                pageResults, continuationToken: continuationToken);

            var commentResponses = _mapper.Map<IEnumerable<GetCommentResponse>>(pageResult.Items);

            int totalPages = (int)Math.Ceiling((double)commentList.Count() / pageResults);

            var pagedComments = new PagedCommentResponse
            {
                Comments = commentResponses,
                CurrentPage = getComment.CurrentPage,
                Pages = totalPages,
                PageSize = getComment.PageSize,
                CommentsCount = commentList.Count(),
            };

            return pagedComments;
        }
    }
}