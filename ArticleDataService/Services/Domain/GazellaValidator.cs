using System.Text.Json;
using ArticleService.Data.Repositories;
using ArticleService.Entities;
using ArticleService.Services.Exceptions;

namespace ArticleService.Services.Domain;

public static class GazellaValidator
{
    private static readonly JsonSerializerOptions Options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
    
    public static bool IsValidGazellaJson(string json)
    {
        try
        {
            var content = JsonSerializer.Deserialize<GazellaContent>(json, Options);

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
        var category = await repository.GetCategoryByIdAsync(categoryId);

        return category as Category ?? throw new GazellaValidationException("Category not found");
    }
}
