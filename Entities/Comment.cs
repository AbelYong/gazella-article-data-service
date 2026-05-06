namespace ArticleService.Entities;

public class Comment
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string ArticleId { get; set; } = string.Empty;
    public string AuthorId { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public string? AuthorProfilePictureUri { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime PostedAt { get; set; }
}
