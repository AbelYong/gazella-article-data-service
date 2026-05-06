namespace ArticleService.Services.Exceptions;

public class GazellaDomainException(string issues) : Exception
{
    public string Issues { get; } = issues;
}
