using ArticleService.Entities;
using ArticleService.Entities.Interfaces;
using ArticleService.Services.DataPackages;

namespace ArticleService.Data.Repositories;

public interface IReviewRepository
{
    Task<PaginationResult<IArticle>> GetArticlesPendingReview(int offset, int pageSize);
    Task<IArticle> GetArticlePendingReview(string id);
    Task<ConcurrentResult<string>> UpdateArticleReview(Article article, ArticleStatus status);
}
