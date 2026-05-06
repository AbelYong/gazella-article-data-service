using System.ComponentModel.DataAnnotations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace ArticleService.Services.Domain;

public record GazellaBlock(
    [property: JsonPropertyName("id")] string? Id,
    [property: JsonPropertyName("type")] [Required] string Type,
    [property: JsonPropertyName("data")] [Required] JsonElement Data);
    