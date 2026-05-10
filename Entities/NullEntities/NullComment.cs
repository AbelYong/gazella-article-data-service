using ArticleService.Entities.Interfaces;

namespace ArticleService.Entities.NullEntities;

public class NullComment : IComment
{
    public string Id => Guid.Empty.ToString();
    public string ArticleId => Guid.Empty.ToString();
    public string AuthorId => Guid.Empty.ToString();
    public string AuthorName => string.Empty;
    public string AuthorProfilePictureUri => string.Empty;
    public string Content => string.Empty;
    public DateTime PostedAt => DateTime.MinValue;
}
