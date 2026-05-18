using ArticleService.Data;

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
    
    public static void ValidateAuthorName(List<string> issues, string name)
    {
        if (string.IsNullOrEmpty(name))
        {
            issues.Add("Author name is required");
        }
        else if (name.Length > EntitySizeConstraints.AuthorNameMaxLength)
        {
            issues.Add($"Author name is too long, max length is {EntitySizeConstraints.AuthorNameMaxLength}");
        }
    }

    public static void ValidateAuthorPfpUri(List<string> issues, string pfpUri)
    {
        if (pfpUri.Length > EntitySizeConstraints.AuthorProfilePictureUriMaxLength)
        {
            issues.Add($"Author profile picture URI is too long, max length is {EntitySizeConstraints.AuthorProfilePictureUriMaxLength}");
        }
        //todo uri format verification
    }
}