namespace ArticleService.Services.DataPackages;

public class TopAuthorArticle
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public int LikeCount { get; set; }
    public int CommentsCount { get; set; }
}
