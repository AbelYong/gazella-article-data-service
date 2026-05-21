using ArticleService.Data.Exceptions;
using ArticleService.Entities;
using ArticleService.Entities.Interfaces;
using ArticleService.Entities.NullEntities;
using ArticleService.Services.DataPackages;
using ArticleService.Services.Domain;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;

namespace ArticleService.Data.Repositories.Implementations;

public class ArticleRepository(GazellaDbContext context, ILogger<CategoryRepository> logger)
    : BaseRepository(context, logger), IArticleRepository
{
    public async Task<bool> VerifyArticleExists(string id)
    {
        try
        {
            var article = await Context.Articles.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);

            return article != null;
        }
        catch (Exception ex) when (ex.InnerException is MongoConnectionException || ex is TimeoutException)
        {
            logger.LogError(ex, "Connection exception while retrieving article {Ex}", ex.Message);
            throw new GazellaDbException(ex.Message, ex);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected exception while retrieving article {Ex}", ex.Message);
            throw new GazellaDbException(ex.Message, ex.InnerException ?? ex);
        }
    }
    
    public async Task<IEnumerable<IArticle>> GetArticlesByAuthorId(string authorId)
    {
        try
        {
            var articles = await Context.Articles
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

    public async Task<IArticle> GetArticleById(string id)
    {
        try
        {
            var article =  await Context.Articles.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);

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

    public async Task<PaginationResult<IArticle>> SearchArticlesAsync(ArticleSearch search, int offset, int pageSize)
    {
        return await ExecuteDbOperationAsync(async () =>
        {
            IQueryable<Article> query = Context.Articles.AsQueryable();

            query = query.Where(a => a.Status == ArticleStatus.Published);

            if (!string.IsNullOrEmpty(search.Title)) 
            {
                query = query.Where(a => a.Title.ToLower().Contains(search.Title));
            }
            if (!string.IsNullOrEmpty(search.Category))
            {
                query = query.Where(a => a.Category.ToLower().Contains(search.Category));
            }
            if (!string.IsNullOrEmpty(search.AuthorName))
            {
                query = query.Where(a => a.Author.Name != null && a.Author.Name.ToLower().Contains(search.AuthorName));
            }
            if (search.PublishedAfter.HasValue)
            {
                query = query.Where(a => a.PublishedAt >= search.PublishedAfter.Value);
            }

            query = search.SortBy switch
            {
                "views" => query.OrderByDescending(a => a.Metrics.Views),
                "comments" => query.OrderByDescending(a => a.Metrics.CommentsCount),
                "likes" => query.OrderByDescending(a => a.Metrics.Likes),
                _ => query.OrderByDescending(a => a.PublishedAt)
            };
            
            var totalItems = await query.CountAsync();
            
            var result = await query
                .Skip(offset)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return new PaginationResult<IArticle>
            {
                Items = result.Cast<IArticle>().ToList(),
                TotalItems = totalItems,
                PageCount = (int)Math.Ceiling(totalItems / (double)pageSize)
            };
        }, "searching articles");
    }

    public async Task<PaginationResult<IArticle>> GetPublishedArticlesAsync(int  offset, int pageSize)
    {
        return await ExecuteDbOperationAsync(async () =>
        {
            var query = Context.Articles.Where(a =>
                a.Status == ArticleStatus.Published || a.Status == ArticleStatus.Removed);
            
            var totalItems =  await query.CountAsync();
            var articles = await query
                .Skip(offset)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();
            
            return new PaginationResult<IArticle>()
            {
                Items = articles.Cast<IArticle>().ToList(),
                TotalItems = articles.Count,
                PageCount = (int)Math.Ceiling(totalItems / (double)pageSize)
            };
        }, "retrieving published articles");
    }

    public async Task<IArticle> RemoveArticleAsync(string id)
    {
        return await ExecuteDbOperationAsync<IArticle>(async () =>
        {
            var article = await Context.Articles
                .Where(a => a.Status == ArticleStatus.Published)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (article == null)
            {
                return new NullArticle();
            }
            
            article.Status = ArticleStatus.Removed;
            await Context.SaveChangesAsync();

            return article;
        }, "Removing article");
    }
}