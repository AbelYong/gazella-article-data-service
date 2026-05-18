using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ArticleService.Entities.Interfaces;

namespace ArticleService.Entities;

public class Article : IArticle
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
    public DateTimeOffset? PublishedAt { get; set; }
    public DateTimeOffset? LastUpdatedAt { get; set; }
    public ArticleStatus Status { get; set; } = ArticleStatus.Draft;
    public string Content { get; set; } = string.Empty;
    public required Author Author { get; set; }
    public Review? ReviewMetadata { get; set; }
    public Metrics Metrics { get; set; } = new();
    public List<RecentComment> RecentComments { get; set; } = new List<RecentComment>();
    
    [NotMapped]
    public string AuthorId => Author.Id;
    [NotMapped]
    public string? AuthorName => Author.Name;
    [NotMapped]
    public string? AuthorProfilePictureUri => Author.ProfilePictureUri;
    [NotMapped]
    public bool? IsApproved => ReviewMetadata?.IsApproved;
    [NotMapped]
    public string? RejectionReason => ReviewMetadata?.RejectionReason;
    [NotMapped]
    public string? ReviewedById => ReviewMetadata?.ReviewedById;
    [NotMapped]
    public DateTimeOffset? ReviewedAt => ReviewMetadata?.ReviewedAt;
    [NotMapped]
    public int Views => Metrics.Views;
    [NotMapped]
    public int Likes => Metrics.Likes;
    [NotMapped]
    public int CommentsCount => Metrics.CommentsCount;
    [NotMapped]
    public IEnumerable<IRecentComment> Comments => RecentComments;
}
