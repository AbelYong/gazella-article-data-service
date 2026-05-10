namespace ArticleService.Entities.Interfaces;

public interface IMetrics
{
    int Views { get; }
    int Likes { get; }
    int CommentsCount { get; }
}