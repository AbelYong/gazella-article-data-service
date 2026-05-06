using ArticleService.Data.Exceptions;
using ArticleService.Entities;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace ArticleService.Data.Repositories.Implementations;

public class DraftRepository(GazellaDbContext context, ILogger<DraftRepository> logger) : IDraftRepository
{
    public async Task<string> SaveDraft(Article draft)
    {
        try
        {
            await context.Articles.AddAsync(draft);
            await  context.SaveChangesAsync();
            
            return draft.Id;
        }
        catch (Exception ex) when (ex.InnerException is MongoConnectionException || ex is TimeoutException)
        {
            logger.LogError(ex, "Connection exception while saving draft {Ex}", ex.Message);
            throw new GazellaDbException(ex.Message, ex);
        }
        catch (Exception ex) when (ex is DbUpdateException || ex.InnerException is DbUpdateConcurrencyException)
        {
            logger.LogError(ex, "DbUpdate exception while saving draft {Ex}", ex.Message);
            throw new GazellaDbException(ex.Message, ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected exception while saving draft: {Ex}", ex.Message);
            throw new GazellaDbException(ex.Message, ex.InnerException ?? ex);
        }
    }
}