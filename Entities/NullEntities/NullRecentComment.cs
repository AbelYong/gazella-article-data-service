using ArticleService.Entities.Interfaces;

namespace ArticleService.Entities.NullEntities;

public class NullRecentComment : IRecentComment
{
    public string Id => Guid.Empty.ToString();
    public string AuthorId =>  string.Empty;
    public string AuthorName =>  string.Empty;
    public string AuthorProfilePictureUri =>  string.Empty;
    public string Content =>  string.Empty;
    public DateTime PostedAt => DateTime.MinValue;
}