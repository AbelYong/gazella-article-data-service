namespace ArticleService.Entities;

public class Review
{
    public string RejectionReason { get; set; } = string.Empty;
    public string? ReviewedById { get; set; }
    public DateTime? ReviewedAt { get; set; }
}
