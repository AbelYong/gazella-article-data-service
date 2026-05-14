using ArticleService.Entities.Interfaces;

namespace ArticleService.Data.Repositories;

public interface ICategoryRepository
{
    Task<ICategory> GetCategoryById(string categoryId);
    Task<IEnumerable<ICategory>> GetCategories();
}