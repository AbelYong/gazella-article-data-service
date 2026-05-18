using ArticleService.Entities.Interfaces;

namespace ArticleService.Entities;

public class Review : IReview
{
    public bool IsApproved { get; set; }
    public string RejectionReason { get; set; } = string.Empty;
    public required string ReviewedById { get; set; }
    public DateTimeOffset ReviewedAt { get; set; }
}
