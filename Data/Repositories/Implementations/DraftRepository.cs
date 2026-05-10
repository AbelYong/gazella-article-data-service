using ArticleService.Data.Exceptions;
using ArticleService.Entities;
using ArticleService.Entities.Interfaces;
using ArticleService.Entities.NullEntities;
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

    public async Task<IArticle> GetExistingDraft(string id)
    {
        try
        {
            var draft = await context.Articles.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);
            
            if (draft != null)
            {
                return draft;
            }
            return new NullArticle();
        }
        catch (Exception ex) when (ex.InnerException is MongoConnectionException || ex is TimeoutException)
        {
            logger.LogError(ex, "Connection exception while verifying if draft exists: {Ex}", ex.Message);
            throw new GazellaDbException(ex.Message, ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected exception while verifying if draft exists: {Ex}", ex.Message);
            throw new GazellaDbException(ex.Message, ex.InnerException ?? ex);
        }
    }
    
    public async Task UpdateDraft(Article draft)
    {
        try
        {
            context.Articles.Update(draft);
            await context.SaveChangesAsync();
        }
        catch (Exception ex) when (ex.InnerException is MongoConnectionException || ex is TimeoutException)
        {
            logger.LogError(ex, "Connection exception while updating draft {Ex}", ex.Message);
            throw new GazellaDbException(ex.Message, ex);
        }
        catch (Exception ex) when (ex is DbUpdateException || ex.InnerException is DbUpdateConcurrencyException)
        {
            logger.LogError(ex, "DbUpdate exception while updating draft {Ex}", ex.Message);
            throw new GazellaDbException(ex.Message, ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected exception while updating draft: {Ex}", ex.Message);
            throw new GazellaDbException(ex.Message, ex.InnerException ?? ex);
        }
    }

    public async Task<string> SaveDraftPublication(Article draft)
    {
        try
        {
            draft.Status = ArticleStatus.UnderReview;
            context.Articles.Update(draft);
            await context.SaveChangesAsync();

            return draft.Status.ToString();
        }
        catch (Exception ex) when (ex.InnerException is MongoConnectionException || ex is TimeoutException)
        {
            logger.LogError(ex, "Connection exception while updating draft {Ex}", ex.Message);
            throw new GazellaDbException(ex.Message, ex);
        }
        catch (Exception ex) when (ex is DbUpdateException || ex.InnerException is DbUpdateConcurrencyException)
        {
            logger.LogError(ex, "DbUpdate exception while updating draft {Ex}", ex.Message);
            throw new GazellaDbException(ex.Message, ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected exception while updating draft: {Ex}", ex.Message);
            throw new GazellaDbException(ex.Message, ex.InnerException ?? ex);
        }
    }
}
