namespace ArticleService.Entities.Interfaces;

public interface ILike
{
    public string Id { get; }
    public string ArticleId { get; }
    public string AuthorId { get; }
    public bool IsLiked { get; }
}
