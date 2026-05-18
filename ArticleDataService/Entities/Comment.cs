using ArticleService.Entities.Interfaces;

namespace ArticleService.Entities;

public class Comment : IComment
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ArticleId { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string? AuthorProfilePictureUri { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTimeOffset PostedAt { get; set; }
}
