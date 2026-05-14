namespace ArticleService.Data.Exceptions;

public class GazellaDbException(string message, Exception inner) : Exception(message, inner);