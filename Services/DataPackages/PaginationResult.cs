namespace ArticleService.Services.DataPackages;

public class PaginationResult<T>
{
    public IList<T> Items { get; set; } = [];
    public int TotalItems { get; set; }
    public int PageCount { get; set; }
}
