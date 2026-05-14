using ArticleService.Data.Exceptions;
using ArticleService.Entities;
using ArticleService.Entities.Interfaces;
using ArticleService.Entities.NullEntities;
using ArticleService.Services.DataPackages;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace ArticleService.Data.Repositories.Implementations;

public class ReviewRepository(GazellaDbContext context, ILogger<ReviewRepository> logger)
    : IReviewRepository
{
    public async Task<PaginationResult<IArticle>> GetArticlesPendingReview(int offset, int pageSize)
    {
        try
        {
            var query = context.Articles
                .Where(a => a.Status == ArticleStatus.UnderReview)
                .OrderByDescending(a => a.LastUpdatedAt);

            var count = await query.CountAsync();
            
            var page = await query.Skip(offset).Take(pageSize).AsNoTracking().ToListAsync();

            var pageCount = (int)Math.Ceiling((double)count / pageSize);

            return new PaginationResult<IArticle>()
            {
                Items = page.Cast<IArticle>().ToList(),
                TotalItems = count,
                PageCount = pageCount,
            };
        }
        catch (Exception ex) when (ex.InnerException is MongoConnectionException || ex is TimeoutException)
        {
            logger.LogError(ex, "Connection exception while querying for articles pending review: {Ex}", ex.Message);
            throw new GazellaDbException(ex.Message, ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected exception while querying for articles pending review: {Ex}", ex.Message);
            throw new GazellaDbException(ex.Message, ex.InnerException ?? ex);
        }
    }

    /// <summary>
    /// Returns an article tracked by the DbContext only if its current state is under review
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <exception cref="GazellaDbException"></exception>
    public async Task<IArticle> GetArticlePendingReview(string id)
    {
        try
        {
            var article =  await context.Articles
                .Where(a => a.Status == ArticleStatus.UnderReview)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (article != null)
            {
                return article;
            }
            return new NullArticle();
        }
        catch (Exception ex) when (ex.InnerException is MongoConnectionException || ex is TimeoutException)
        {
            logger.LogError(ex, "Connection exception while retrieving article by Id {Ex}", ex.Message);
            throw new GazellaDbException(ex.Message, ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected exception while retrieving article by Id: {Ex}", ex.Message);
            throw new GazellaDbException(ex.Message, ex.InnerException ?? ex);
        }
    }

    /// <summary>
    /// Updates the Review for the given article, the received article should be tracked by the context
    /// </summary>
    /// <param name="article"></param>
    /// <param name="status"></param>
    /// <returns></returns>
    /// <exception cref="GazellaDbException"></exception>
    public async Task<ConcurrentResult<string>> UpdateArticleReview(Article article, ArticleStatus status)
    {
        try
        {
            article.Status = status;

            await using (context)
            {
                await context.SaveChangesAsync();
            }

            return new ConcurrentResult<string>
            {
                ConcurrencyTokenField = status.ToString(),
                IsSuccess = true,
            };
        }
        catch (DbUpdateConcurrencyException)
        {
            return new ConcurrentResult<string>
            {
                ConcurrencyTokenField = status.ToString(),
                IsSuccess = false,
            };
        }
        catch (Exception ex) when (ex.InnerException is MongoConnectionException || ex is TimeoutException)
        {
            logger.LogError(ex, "Connection exception while updating article review {Ex}", ex.Message);
            throw new GazellaDbException(ex.Message, ex);
        }
        catch (Exception ex) when (ex is DbUpdateException || ex.InnerException is DbUpdateConcurrencyException)
        {
            logger.LogError(ex, "DbUpdate exception while updating article review {Ex}", ex.Message);
            throw new GazellaDbException(ex.Message, ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected exception while updating article review {Ex}", ex.Message);
            throw new GazellaDbException(ex.Message, ex.InnerException ?? ex);
        }
    }
}