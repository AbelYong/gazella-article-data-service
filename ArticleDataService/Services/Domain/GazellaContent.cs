using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace ArticleService.Services.Domain;

public record GazellaContent(
    [property: JsonPropertyName("time")] long Time,
    [property: JsonPropertyName("blocks")] [Required] List<GazellaBlock> Blocks,
    [property: JsonPropertyName("version")] string Version
    );