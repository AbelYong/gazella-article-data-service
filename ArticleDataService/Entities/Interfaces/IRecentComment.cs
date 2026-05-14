namespace ArticleService.Entities.Interfaces;

public interface IRecentComment
{
    string Id { get; }
    string AuthorId { get; }
    string AuthorName { get; }
    string? AuthorProfilePictureUri { get; }
    string Content { get; }
    DateTime PostedAt { get; }
}
