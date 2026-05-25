using ArticleService.Entities.Interfaces;
using ArticleService.Services.DataPackages;
using ArticleService.Services.Domain;

namespace ArticleService.Data.Repositories;

public interface IArticleRepository
{
    Task<bool> VerifyArticleExistsAsync(string id);
    Task<IEnumerable<IArticle>> GetArticlesByAuthorIdAsync(string authorId);
    Task<IArticle> GetArticleByIdAsync(string id);
    Task<PaginationResult<IArticle>> SearchArticlesAsync(ArticleSearch search, int offset, int pageSize);
    Task<PaginationResult<IArticle>> GetPublishedArticlesAsync(int offset, int pageSize);
    Task<IArticle> RemoveArticleAsync(string id);
    Task<AuthorStats> GetAuthorStatsAsync(string authorId);
    Task<IEnumerable<IArticle>> GetFeaturedArticlesAsync(int amount);
}
