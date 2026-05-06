using System.ComponentModel.DataAnnotations;

namespace ArticleService.Entities;

public class Author
{
    public string Id { get; set; } = string.Empty;
    public string? Name { get; set; } = string.Empty;
    public string? ProfilePictureUri { get; set; }
}
