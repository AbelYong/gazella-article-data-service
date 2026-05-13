namespace ArticleService.Services.DataPackages;

public class ConcurrentResult<T>
{
    public required T ConcurrencyTokenField { get; set; }
    public bool IsSuccess { get; set;}
}