using ArticleService.Entities;
using ArticleService.Entities.Interfaces;
using ArticleService.Entities.NullEntities;
using ArticleService.Services.DataPackages;
using ArticleService.Services.Domain;
using Microsoft.EntityFrameworkCore;

namespace ArticleService.Data.Repositories.Implementations;

public class ArticleRepository(GazellaDbContext context, ILogger<ArticleRepository> logger)
    : BaseRepository(context, logger), IArticleRepository
{
    public async Task<bool> VerifyArticleExistsAsync(string id)
    {
        return await ExecuteDbOperationAsync(async () =>
        {
            var article = await Context.Articles.AsNoTracking().FirstOrDefaultAsync(a => a.Id == id);

            return article != null;
        }, "verifying article exists");
    }

    public async Task<IEnumerable<IArticle>> GetArticlesByAuthorIdAsync(string authorId)
    {
        return await ExecuteDbOperationAsync<IEnumerable<IArticle>>(async () =>
        {
            var articles = await Context.Articles
                .Where(a => a.Author.Id == authorId).AsNoTracking().ToListAsync();

            if (articles.Count > 0)
            {
                return articles;
            }
            return new List<NullArticle>();
        }, "retrieving articles by author id");
    }

    public async Task<IArticle> GetArticleByIdAsync(string id)
    {
        return await ExecuteDbOperationAsync<IArticle>(async () =>
        {
            var article = await Context.Articles.FirstOrDefaultAsync(a => a.Id == id);

            if (article == null)
            {
                return new NullArticle();
            }

            if (article.Status == ArticleStatus.Published)
            {
                article.Metrics.Views++;
                await Context.SaveChangesAsync();
            }

            return article;
        }, "retrieving article by id");
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

    public async Task<PaginationResult<IArticle>> GetPublishedArticlesAsync(int offset, int pageSize)
    {
        return await ExecuteDbOperationAsync(async () =>
        {
            var query = Context.Articles.Where(a =>
                a.Status == ArticleStatus.Published || a.Status == ArticleStatus.Removed);

            var totalItems = await query.CountAsync();
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
    
    public async Task<AuthorStats> GetAuthorStatsAsync(string authorId)
    {
        return await ExecuteDbOperationAsync(async () =>
        {
            var stats = new AuthorStats { TopAuthorArticles = new List<TopAuthorArticle>() };

            var articleMetrics = await Context.Articles
                .AsNoTracking()
                .Where(a => a.Author.Id == authorId && a.Status == ArticleStatus.Published)
                .OrderByDescending(a => a.Metrics.Likes)
                .Select(a => new
                {
                    a.Id,
                    a.Title,
                    a.Metrics.Likes,
                    a.Metrics.CommentsCount,
                    a.Metrics.Views
                })
                .ToListAsync();

            if (articleMetrics.Count <= 0)
            {
                return stats;
            }

            stats.TopAuthorArticles = articleMetrics.Select(a => new TopAuthorArticle
            {
                Id = a.Id,
                Title = a.Title,
                CommentsCount = a.CommentsCount,
                LikeCount = a.Likes
            }).Take(3).ToList();

            var articles = await Context.Articles
                .Where(a => a.Author.Id == authorId)
                .AsNoTracking()
                .ToListAsync();

            var recentComments = articles
                .SelectMany(a => a.RecentComments)
                .ToList();

            var orderedRecentComments = recentComments.Select(c => new
            {
                CommentId = c.Id,
                ArticleId = c.Id,
                c.PostedAt
            }).OrderByDescending(a => a.PostedAt).ToList();

            var latestCommentId = string.Empty;
            var latestCommentArticleId = string.Empty;
            var latestCommentPostedAt = DateTime.MinValue;

            if (orderedRecentComments.Count > 0)
            {
                latestCommentId = orderedRecentComments[0].CommentId;
                latestCommentArticleId = orderedRecentComments[0].ArticleId;
                latestCommentPostedAt = orderedRecentComments[0].PostedAt;
            }

            var articleIds = articleMetrics.Select(a => a.Id).ToList();
            var startOfTodayUtc = DateTime.UtcNow.Date;

            var likesToday = await Context.Likes
                .Where(l => articleIds.Contains(l.ArticleId)
                            && l.LikedAt >= startOfTodayUtc
                            && l.IsLiked)
                .CountAsync();

            stats.RecentActivity = new RecentActivity
            {
                LatestCommentId = latestCommentId,
                LatestCommentArticleId = latestCommentArticleId,
                LatestCommentPostedAt = latestCommentPostedAt,
                LikesToday = likesToday
            };

            stats.TotalLikes = articleMetrics.Sum(a => a.Likes);
            stats.TotalComments = articleMetrics.Sum(a => a.CommentsCount);
            stats.PublishedArticlesCount = articleMetrics.Count;

            var validArticlesForEngagement = articleMetrics.Where(a => a.Views > 0).ToList();

            var engagementRate = validArticlesForEngagement.Count != 0
                ? validArticlesForEngagement.Average(a => (float)(a.Likes + a.CommentsCount) / a.Views)
                : 0;

            stats.EngagementRate = engagementRate;

            return stats;
        }, "retrieving author stats");
    }
    
    public async Task<IEnumerable<IArticle>> GetFeaturedArticlesAsync(int amount)
    {
        return await ExecuteDbOperationAsync(async () =>
        {
            var featured = await Context.Articles
                .OrderByDescending(a => a.PublishedAt)
                .Take(amount)
                .AsNoTracking()
                .ToListAsync();
            
            if (featured.Count  == 0)
            {
                return new List<NullArticle>();
            }

            featured = featured.OrderByDescending(a => a.Metrics.Views)
                .ThenByDescending(a => a.Metrics.CommentsCount)
                .ThenByDescending(a => a.Metrics.Likes)
                .ToList();

            return featured.Cast<IArticle>();
        }, "retrieving feature articles");
    }
}