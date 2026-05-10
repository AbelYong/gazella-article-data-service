using ArticleService.Entities.Interfaces;

namespace ArticleService.Data.Repositories;

public interface IArticlesRepository
{
    Task<IEnumerable<IArticle>> GetArticlesByAuthorId(string authorId);
}


