namespace ArticleService.Entities.Interfaces;

public interface IArticle
{
    string Id { get; }
    string Title { get; }
    string? CoverUri { get; }
    string? Summary { get; }
    string Category { get; }
    DateTimeOffset? PublishedAt { get; }
    DateTimeOffset? LastUpdatedAt { get; }
    ArticleStatus Status { get; }
    string Content { get; }
    string AuthorId { get; }
    string? AuthorName { get; }
    string? AuthorProfilePictureUri { get; }
    bool? IsApproved { get; }
    string? RejectionReason { get; }
    string? ReviewedById { get; }
    DateTimeOffset? ReviewedAt { get; }
    int Views { get; }
    int Likes { get; }
    int CommentsCount { get; }
    IEnumerable<IRecentComment> Comments { get; }
}
