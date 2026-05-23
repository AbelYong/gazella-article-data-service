namespace ArticleService.Services.DataPackages;

public class AuthorStats
{
    public required IEnumerable<TopAuthorArticle> TopAuthorArticles { get; set; }
    public RecentActivity RecentActivity { get; set; } = new RecentActivity();
    public int TotalLikes { get; set; }
    public int TotalComments { get; set; }
    public int PublishedArticlesCount { get; set; }
    public float EngagementRate { get; set; }
}
