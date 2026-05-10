using System.Text.Json;
using ArticleService.Data.Repositories;
using ArticleService.Entities;
using ArticleService.Services.Exceptions;

namespace ArticleService.Services.Domain;

public static class GazellaValidator
{
    
    public static bool IsValidGazellaJson(string json)
    {
        try
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var content = JsonSerializer.Deserialize<GazellaContent>(json, options);

            return content != null && 
                   content.Blocks.All(b => b.Data.ValueKind != JsonValueKind.Undefined);
        }
        catch (JsonException)
        {
            return false;
        }
    }

    public static async Task<Category> VerifyExistingCategory(ICategoryRepository repository, string categoryId)
    {
        var category = await repository.GetCategoryById(categoryId);

        return category as Category ?? throw new GazellaDomainException("Category not found");
    }
}
