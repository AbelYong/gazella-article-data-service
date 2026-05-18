using ArticleService.Entities.Interfaces;

namespace ArticleService.Entities.NullEntities;

public class NullLike : ILike
{
    public string Id => Guid.Empty.ToString();
    public string ArticleId => Guid.Empty.ToString();
    public string AuthorId => Guid.Empty.ToString();
    public bool IsLiked => false;
}
