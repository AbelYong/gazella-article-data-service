using ArticleService.Entities.Interfaces;

namespace ArticleService.Data.Repositories;

public interface ICategoryRepository
{
    Task<ICategory> GetCategoryByIdAsync(string categoryId);
    Task<IEnumerable<ICategory>> GetCategoriesAsync();
}