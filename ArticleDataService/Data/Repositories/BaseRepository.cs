using ArticleService.Data.Exceptions;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace ArticleService.Data.Repositories;

public abstract class BaseRepository(GazellaDbContext context, ILogger logger)
{
    protected readonly GazellaDbContext Context = context;

    protected async Task<T> ExecuteDbOperationAsync<T>(Func<Task<T>> operation, string operationContext)
    {
        try
        {
            return await operation();
        }
        catch (Exception ex) when (ex.InnerException is MongoConnectionException || ex is TimeoutException)
        {
            logger.LogError(ex, "Connection exception while {Context}: {Ex}", operationContext, ex.Message);
            throw new GazellaDbException(ex.Message, ex);
        }
        catch (Exception ex) when (ex is DbUpdateException || ex.InnerException is DbUpdateConcurrencyException)
        {
            logger.LogError(ex, "DbUpdate exception while {Context}: {Ex}", operationContext, ex.Message);
            throw new GazellaDbException(ex.Message, ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected exception while {Context}: {Ex}", operationContext, ex.Message);
            throw new GazellaDbException(ex.Message, ex.InnerException ?? ex);
        }
    }
    
    protected async Task ExecuteDbOperationAsync(Func<Task> operation, string operationContext)
    {
        await ExecuteDbOperationAsync<object>(async () =>
        {
            await operation();
            return null!;
        }, operationContext);
    }
}