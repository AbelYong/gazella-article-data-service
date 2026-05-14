namespace ArticleService.Entities.Interfaces;

public interface IAuthor
{
    string Id { get; }
    string? Name { get; }
    string? ProfilePictureUri { get; }
}