namespace ArticleService.Services.DataPackages;

public class RecentActivity
{
    public string LatestCommentId { get; set; } = string.Empty;
    public string LatestCommentArticleId { get; set; } = string.Empty;
    public DateTime LatestCommentPostedAt { get; set; }
    public int LikesToday { get; set; }
}
