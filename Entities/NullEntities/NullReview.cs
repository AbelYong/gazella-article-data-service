using ArticleService.Entities.Interfaces;

namespace ArticleService.Entities.NullEntities;

public class NullReview : IReview
{
    public bool IsApproved => false;
    public string RejectionReason =>  string.Empty;
    public string ReviewedById =>  string.Empty;
    public DateTime ReviewedAt => DateTime.MinValue;
}
