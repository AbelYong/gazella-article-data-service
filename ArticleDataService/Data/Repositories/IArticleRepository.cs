using ArticleService.Entities.Interfaces;
using ArticleService.Services.DataPackages;
using ArticleService.Services.Domain;

namespace ArticleService.Data.Repositories;

public interface IArticleRepository
{
    Task<bool> VerifyArticleExists(string id);
    Task<IEnumerable<IArticle>> GetArticlesByAuthorId(string authorId);
    Task<IArticle> GetArticleById(string id);
    Task<PaginationResult<IArticle>> SearchArticlesAsync(ArticleSearch search, int offset, int pageSize);
    Task<PaginationResult<IArticle>> GetPublishedArticlesAsync(int offset, int pageSize);
    Task<IArticle> RemoveArticleAsync(string id);
    Task<AuthorStats> GetAuthorStatsAsync(string authorId);
}
