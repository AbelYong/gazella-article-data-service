using ArticleService.Entities.Interfaces;

namespace ArticleService.Entities.NullEntities;

public class NullCategory : ICategory
{
    public string Id => "";
    public string Name => "";
}