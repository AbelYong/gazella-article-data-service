namespace ArticleService.Services.MessageValidators;

public static class GeneralValidator
{
    public static void ValidateId(List<string> issues, string id, string idName)
    {
        if (string.IsNullOrEmpty(id))
        {
            issues.Add($"{idName} is required");
        }
        else if (!Guid.TryParse(id, out _))
        {
            issues.Add($"{idName} is not valid UUID");
        }
    }
}