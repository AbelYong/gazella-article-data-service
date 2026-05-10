using ArticleService.Data.Exceptions;
using ArticleService.Entities.Interfaces;
using ArticleService.Entities.NullEntities;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace ArticleService.Data.Repositories.Implementations;

public class ArticleRepository(GazellaDbContext context, ILogger<CategoryRepository> logger) : IArticlesRepository
{
    public async Task<IEnumerable<IArticle>> GetArticlesByAuthorId(string authorId)
    {
        try
        {
            var articles = await context.Articles
                .Where(a => a.Author.Id == authorId).AsNoTracking().ToListAsync();
            
            if (articles.Count > 0)
            {
                return articles;
            }
            return new List<NullArticle>();
        }
        catch (Exception ex) when (ex.InnerException is MongoConnectionException || ex is TimeoutException)
        {
            logger.LogError(ex, "Connection exception while querying articles by author Id {Ex}", ex.Message);
            throw new GazellaDbException(ex.Message, ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected exception while querying articles by author Id: {Ex}", ex.Message);
            throw new GazellaDbException(ex.Message, ex.InnerException ?? ex);
        }
    }
}