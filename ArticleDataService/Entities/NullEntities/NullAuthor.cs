using ArticleService.Entities.Interfaces;

namespace ArticleService.Entities.NullEntities;

public class NullAuthor : IAuthor
{
    public string Id => Guid.Empty.ToString();
    public string Name => string.Empty;
    public string ProfilePictureUri => string.Empty;
}
