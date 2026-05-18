using ArticleService.Entities.Interfaces;

namespace ArticleService.Entities;

public class RecentComment : IRecentComment
{
    public string Id { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string? AuthorProfilePictureUri { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTimeOffset PostedAt { get; set; }
}
