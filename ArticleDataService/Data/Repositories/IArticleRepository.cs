using ArticleService.Entities.Interfaces;

namespace ArticleService.Data.Repositories;

public interface IArticleRepository
{
    Task<bool> VerifyArticleExists(string id);
    Task<IEnumerable<IArticle>> GetArticlesByAuthorId(string authorId);
    Task<IArticle> GetArticleById(string id);
}
