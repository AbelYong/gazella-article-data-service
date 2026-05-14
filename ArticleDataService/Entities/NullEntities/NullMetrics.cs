using ArticleService.Entities.Interfaces;

namespace ArticleService.Entities.NullEntities;

public class NullMetrics : IMetrics
{
    public int Views => 0;
    public int Likes => 0;
    public int CommentsCount => 0;
}