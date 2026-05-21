namespace ArticleService.Services.Domain;

public class ArticleSearch
{
    public string Title  { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string AuthorName { get; set; } = string.Empty;
    public DateTime? PublishedAfter { get; set; }
    public string SortBy { get; set; } = string.Empty;
}
