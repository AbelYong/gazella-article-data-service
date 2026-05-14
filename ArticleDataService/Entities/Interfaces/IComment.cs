namespace ArticleService.Entities.Interfaces;

public interface IComment
{
    string Id { get; }
    string ArticleId { get; }
    string AuthorId { get; }
    string AuthorName { get; }
    string? AuthorProfilePictureUri { get; }
    string Content { get; }
    DateTime PostedAt { get; }
}