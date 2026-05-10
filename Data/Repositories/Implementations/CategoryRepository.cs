using ArticleService.Data.Exceptions;
using ArticleService.Entities.Interfaces;
using ArticleService.Entities.NullEntities;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace ArticleService.Data.Repositories.Implementations;

public class CategoryRepository(GazellaDbContext context, ILogger<CategoryRepository> logger) : ICategoryRepository
{
    public async Task<ICategory> GetCategoryById(string categoryId)
    {
        try
        {
            var category = await context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == categoryId);
            if (category != null)
            {
                return category;
            }
            return new NullCategory();
        }
        catch (Exception ex) when (ex.InnerException is MongoConnectionException || ex is TimeoutException)
        {
            logger.LogError(ex, "Connection exception while querying by category id {Ex}", ex.Message);
            throw new GazellaDbException(ex.Message, ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected exception while querying by category id: {Ex}", ex.Message);
            throw new GazellaDbException(ex.Message, ex.InnerException ?? ex);
        }
    }

    public async Task<IEnumerable<ICategory>> GetCategories()
    {
        try
        {
            var categories = await context.Categories.AsNoTracking().ToListAsync();
            return categories;
        }
        catch (Exception ex) when (ex.InnerException is MongoConnectionException || ex is TimeoutException)
        {
            logger.LogError(ex, "Connection exception while retrieving all categories: {Ex}", ex.Message);
            throw new GazellaDbException(ex.Message, ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected exception while retrieving all categories: {Ex}", ex.Message);
            throw new GazellaDbException(ex.Message, ex.InnerException ?? ex);
        }
    }
}
