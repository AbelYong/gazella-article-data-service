namespace ArticleService.Entities.Interfaces;

public interface IReview
{
    public string RejectionReason { get; }
    public string ReviewedById { get; }
    public DateTime ReviewedAt { get; }
}
