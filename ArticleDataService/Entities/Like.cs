using ArticleService.Entities.Interfaces;

namespace ArticleService.Entities;

public class Like : ILike
{
    public string Id { get; } = Guid.NewGuid().ToString();
    public required string ArticleId { get; set; }
    public required string AuthorId { get; set; }
    public bool IsLiked { get; set; }
}
