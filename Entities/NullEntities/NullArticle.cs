using ArticleService.Entities.Interfaces;

namespace ArticleService.Entities.NullEntities;

public class NullArticle : IArticle
{
    public string Id => Guid.Empty.ToString();
    public string Title =>  string.Empty;
    public string CoverUri =>  string.Empty;
    public string Summary =>  string.Empty;
    public string Category => string.Empty;
    public DateTime? PublishedAt => DateTime.MinValue;
    public DateTime? LastUpdatedAt => DateTime.MinValue;
    public ArticleStatus Status => ArticleStatus.Draft;
    public string Content => string.Empty;
    
    public string AuthorId => string.Empty;
    public string AuthorName => string.Empty;
    public string AuthorProfilePictureUri => string.Empty;
    public string RejectionReason => string.Empty;
    public string ReviewedById => Guid.Empty.ToString();
    public DateTime? ReviewedAt => DateTime.MinValue;
    public int Views => 0;
    public int Likes => 0;
    public int CommentsCount => 0;
    public IEnumerable<IRecentComment> Comments { get; } = new List<NullRecentComment>().Cast<IRecentComment>().ToList();
}
