using ArticleService.Entities;
using ArticleService.Entities.Interfaces;
using ArticleService.Entities.NullEntities;
using Microsoft.EntityFrameworkCore;

namespace ArticleService.Data.Repositories.Implementations;

public class CategoryRepository(GazellaDbContext context, ILogger<CategoryRepository> logger) 
    : BaseRepository(context, logger), ICategoryRepository
{
    public async Task<Category> AddCategoryAsync(Category category)
    {
        return await ExecuteDbOperationAsync(async () =>
        {
            Context.Categories.Add(category);
            await Context.SaveChangesAsync();
            return category;
        }, "adding category");
    } 
    
    public async Task<ICategory> GetCategoryByIdAsync(string categoryId)
    {
        return await ExecuteDbOperationAsync<ICategory>(async () =>
        {
            var category = await Context.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == categoryId);
            if (category != null)
            {
                return category;
            }
            return new NullCategory();
        },"retrieving category by Id");
    }

    public async Task<IEnumerable<ICategory>> GetCategoriesAsync()
    {
        return await ExecuteDbOperationAsync<IEnumerable<ICategory>>(async () =>
        {
            var categories = await Context.Categories.AsNoTracking().ToListAsync();
            return categories;
        }, "retrieving  all categories");
    }

    public async Task DeleteAllCategories()
    {
        await ExecuteDbOperationAsync(async () =>
        {
            await foreach (var category in Context.Categories)
            {
                Context.Categories.Remove(category);
            }
            await Context.SaveChangesAsync();
        }, "deleting all categories");
    }
}
