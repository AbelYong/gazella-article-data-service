using ArticleService.Entities.Interfaces;

namespace ArticleService.Entities;

public class Category : ICategory
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public string Name { get; set; } = string.Empty;
}
