using System.ComponentModel.DataAnnotations;
using MongoDB.Bson;

namespace ArticleService.Entities;

public class Article
{
    [MaxLength(36)]
    public string Id { get; init; } = Guid.NewGuid().ToString();
    [MaxLength(128)]
    public string Title { get; set; } = string.Empty;
    [MaxLength(256)]
    public string? CoverUri { get; set; }
    [MaxLength(500)]
    public string? Summary { get; set; }
    [MaxLength(64)]
    public string Category { get; set; } = string.Empty;
    public DateTime? PublishedAt { get; set; }
    public DateTime? LastUpdatedAt { get; set; }
    public ArticleStatus Status { get; set; } = ArticleStatus.Draft;
        
    public Author Author { get; set; } = null!;
    public Review? ReviewMetadata { get; set; }
    public Metrics Metrics { get; set; } = new();
    public string Content { get; set; } = string.Empty;
        
    public List<RecentComment> RecentComments { get; set; } = new();
}
