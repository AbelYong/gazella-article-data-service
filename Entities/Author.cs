using ArticleService.Entities.Interfaces;

namespace ArticleService.Entities;

public class Author : IAuthor
{
    public string Id { get; set; } = string.Empty;
    public string? Name { get; set; } = string.Empty;
    public string? ProfilePictureUri { get; set; }
}
