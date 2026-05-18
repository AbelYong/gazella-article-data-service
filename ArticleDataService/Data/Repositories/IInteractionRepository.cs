using ArticleService.Entities;
using ArticleService.Entities.Interfaces;
using ArticleService.Services.DataPackages;

namespace ArticleService.Data.Repositories;

public interface IInteractionRepository
{
    Task<IRecentComment> AddCommentAsync(string articleId, Comment comment);
    Task<IComment>  DeleteCommentAsync(string articleId, string commentId);
    Task<PaginationResult<IComment>> GetCommentsAsync(string articleId, int offset, int pageSize);
    Task<ILike> GetExistingLikeAsync(string articleId, string authorId);
    Task<UpdateLikeResult> LikeArticleAsync(string articleId, string authorId);
    Task<UpdateLikeResult> RevokeLikeAsync(string articleId, string likeId);
}
