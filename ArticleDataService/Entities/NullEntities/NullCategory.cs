using ArticleService.Entities.Interfaces;

namespace ArticleService.Entities.NullEntities;

public class NullCategory : ICategory
{
    public string Id => Guid.Empty.ToString();
    public string Name => string.Empty;
}
