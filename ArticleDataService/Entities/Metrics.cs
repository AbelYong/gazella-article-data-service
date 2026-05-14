using ArticleService.Entities.Interfaces;

namespace ArticleService.Entities;

public class Metrics : IMetrics
{
    public int Views { get; set; }
    public int Likes { get; set; }
    public int CommentsCount { get; set; }
}