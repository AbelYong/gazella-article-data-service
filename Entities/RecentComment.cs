namespace ArticleService.Entities;

public class RecentComment
{
    public string Id { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string? AuthorProfilePictureUri { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime PostedAt { get; set; }
}
