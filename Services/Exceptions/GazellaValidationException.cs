namespace ArticleService.Services.Exceptions;

public class GazellaValidationException(string issues) : Exception
{
    public string Issues { get; } = issues;
}
