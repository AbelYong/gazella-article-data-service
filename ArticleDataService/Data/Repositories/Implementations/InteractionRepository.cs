using ArticleService.Entities;
using ArticleService.Entities.Interfaces;
using ArticleService.Entities.NullEntities;
using ArticleService.Services.DataPackages;
using Microsoft.EntityFrameworkCore;

namespace ArticleService.Data.Repositories.Implementations;

public class InteractionRepository(GazellaDbContext context, ILogger<BaseRepository> logger)
    : BaseRepository(context, logger), IInteractionRepository
{
    public async Task<IRecentComment> AddCommentAsync(string articleId, Comment comment)
    {
        return await ExecuteDbOperationAsync<IRecentComment>(async () =>
        {
            await using (Context)
            {
                var article = await Context.Articles.FindAsync(articleId);

                if (article == null)
                {
                    return new NullRecentComment();
                }

                var recentComment = new RecentComment
                {
                    Id = comment.Id,
                    AuthorId =  comment.AuthorId,
                    AuthorName = comment.AuthorName,
                    AuthorProfilePictureUri =  comment.AuthorProfilePictureUri,
                    Content = comment.Content,
                    PostedAt = comment.PostedAt
                };
        
                article.RecentComments.Insert(0, recentComment);
                article.Metrics.CommentsCount++;
        
                if (article.RecentComments.Count > EntitySizeConstraints.RecentCommentsLimit)
                {
                    var oldestRecent = article.RecentComments[^1];
                    article.RecentComments.Remove(oldestRecent);

                    var overflowComment = new Comment
                    {
                        Id = oldestRecent.Id,
                        ArticleId = article.Id,
                        AuthorId = oldestRecent.AuthorId,
                        AuthorName = oldestRecent.AuthorName,
                        AuthorProfilePictureUri = oldestRecent.AuthorProfilePictureUri,
                        Content = oldestRecent.Content,
                        PostedAt = oldestRecent.PostedAt
                    };
            
                    Context.Comments.Add(overflowComment);
                }
        
                await Context.SaveChangesAsync();

                return recentComment;
            }
        }, "adding comment");
    }

    public async Task<IComment> DeleteCommentAsync(string articleId, string commentId)
    {
        return await ExecuteDbOperationAsync<IComment>(async () =>
        {
            await using (Context)
            {
                var article = await Context.Articles.FindAsync(articleId);

                if (article == null)
                {
                    return new NullComment();
                }

                var isDeleted = false;
                var embeddedComment = article.RecentComments.FirstOrDefault(c => c.Id == commentId);
                var deletedComment = new Comment { Id = Guid.Empty.ToString() };
                
                if (embeddedComment != null)
                {
                    article.RecentComments.Remove(embeddedComment);
                    isDeleted = true;
                    deletedComment.Id = embeddedComment.Id;

                    var replacementComment = await Context.Comments
                        .Where(c => c.ArticleId == articleId)
                        .OrderByDescending(c => c.PostedAt)
                        .FirstOrDefaultAsync();

                    if (replacementComment != null)
                    {
                        article.RecentComments.Add(new RecentComment
                        {
                            Id = replacementComment.Id,
                            AuthorId = replacementComment.AuthorId,
                            AuthorName = replacementComment.AuthorName,
                            AuthorProfilePictureUri = replacementComment.AuthorProfilePictureUri,
                            Content = replacementComment.Content,
                            PostedAt = replacementComment.PostedAt
                        });
                        
                        article.RecentComments = article.RecentComments.OrderByDescending(c => c.PostedAt).ToList();

                        Context.Comments.Remove(replacementComment);
                    }
                }
                else
                {
                    var dbComment = await Context.Comments.FindAsync(commentId);
                    if (dbComment != null && dbComment.ArticleId == articleId)
                    {
                        Context.Comments.Remove(dbComment);
                        
                        isDeleted = true;
                        deletedComment.Id = dbComment.Id;
                    }
                }

                if (!isDeleted)
                {
                    return new NullComment();
                }
                
                article.Metrics.CommentsCount = Math.Max(0, article.Metrics.CommentsCount - 1);
                await Context.SaveChangesAsync();

                return deletedComment;
            }
        }, "deleting comment");
    }

    public async Task<PaginationResult<IComment>> GetCommentsAsync(string articleId, int offset, int pageSize)
    {
        return await ExecuteDbOperationAsync(async () =>
        {
            var query = Context.Comments.Where(c => c.ArticleId == articleId);
            
            var totalItems = await query.CountAsync();

            var items = await query
                .OrderByDescending(c => c.PostedAt)
                .Skip(offset)
                .Take(pageSize)
                .AsNoTracking()
                .ToListAsync();

            return new PaginationResult<IComment>
            {
                Items = items.Cast<IComment>().ToList(),
                TotalItems = totalItems,
                PageCount = (int)Math.Ceiling(totalItems / (double)pageSize)
            };
        }, "retrieving comments");
    }

    public async Task<ILike> GetExistingLikeAsync(string articleId, string authorId)
    {
        return await ExecuteDbOperationAsync<ILike>(async () =>
        {
            var like = await Context.Likes
                .Where(l => l.ArticleId == articleId && l.AuthorId == authorId)
                .AsNoTracking()
                .FirstOrDefaultAsync();
            if (like != null)
            {
                return like;
            }
            return new NullLike();
        }, "retrieving existing like");
    }

    public async Task<UpdateLikeResult> LikeArticleAsync(string articleId, string authorId)
    {
        return await ExecuteDbOperationAsync(async () =>
        {
            await using (Context)
            {
                var article = await Context.Articles.FindAsync(articleId);
                if (article == null)
                {
                    return new UpdateLikeResult
                    {
                        Like = new NullLike(),
                        CurrentLikes = 0
                    };
                }

                var like = new Like
                {
                    ArticleId = articleId,
                    AuthorId = authorId,
                    IsLiked = true,
                    LikedAt = DateTime.UtcNow
                };
                
                var currentLikes = article.Metrics.Likes + 1;
                
                Context.Likes.Add(like);
                article.Metrics.Likes++;
                await Context.SaveChangesAsync();
                
                return new UpdateLikeResult
                {
                    Like = like,
                    CurrentLikes = currentLikes
                };
            }
        }, "adding like to article");
    }

    public async Task<UpdateLikeResult> RevokeLikeAsync(string articleId, string likeId)
    {
        return await ExecuteDbOperationAsync(async () =>
        {
            var defaultResult = new UpdateLikeResult
            {
                Like = new NullLike(),
                CurrentLikes = 0
            };
            
            await using (Context)
            {
                var article = await Context.Articles.FindAsync(articleId);
                if (article == null)
                {
                    return defaultResult;
                }
            
                var likeToRevoke = await Context.Likes.FindAsync(likeId);
                if (likeToRevoke == null)
                {
                    return defaultResult;
                }
            
                var currentLikes = Math.Max(0, article.Metrics.Likes - 1);
                
                Context.Likes.Remove(likeToRevoke);
                article.Metrics.Likes = currentLikes;
                await Context.SaveChangesAsync();
            
                return new UpdateLikeResult
                {
                    Like = likeToRevoke,
                    CurrentLikes = currentLikes
                };
            }
        }, "revoking like from article");
    }
}
