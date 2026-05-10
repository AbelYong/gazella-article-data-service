using ArticleService.Entities.Interfaces;

namespace ArticleService.Entities;

public class Review : IReview
{
    public string RejectionReason { get; set; } = string.Empty;
    public required string ReviewedById { get; set; }
    public DateTime ReviewedAt { get; set; }
}
