using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;
using ArticleService.Entities;

namespace ArticleService.Data;

public class GazellaDbContext : DbContext
{
    public DbSet<Article> Articles { get; init; }
    public DbSet<Category> Categories { get; init; }
    public DbSet<Comment> Comments { get; init; }
    
    public GazellaDbContext(DbContextOptions<GazellaDbContext> options) 
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Article>(entity =>
            {
                entity.ToCollection("articles");
                
                entity.HasKey(a => a.Id);
                entity.Property(a => a.Id).HasElementName("_id");
                
                entity.Property(a => a.Status)
                      .HasConversion<string>()
                      .HasElementName("status");

                entity.OwnsOne(a => a.Author, author => 
                {
                    author.Property(a => a.Id).HasElementName("id").HasMaxLength(36);
                    author.Property(a => a.Name).HasElementName("name").HasMaxLength(128);
                    author.Property(a => a.ProfilePictureUri).HasElementName("profile_picture_uri").HasMaxLength(256);
                });

                entity.OwnsOne(a => a.ReviewMetadata, review =>
                {
                    review.Property(r => r.RejectionReason).HasElementName("rejection_reason").HasMaxLength(1000).IsRequired();
                    review.Property(r => r.ReviewedById).HasElementName("reviewed_by_id").HasMaxLength(36).IsRequired();
                    review.Property(r => r.ReviewedAt).HasElementName("reviewed_at");
                });

                entity.OwnsOne(a => a.Metrics, metrics =>
                {
                    metrics.Property(m => m.Views).HasElementName("views");
                    metrics.Property(m => m.Likes).HasElementName("likes");
                    metrics.Property(m => m.CommentsCount).HasElementName("comments_count");
                });

                entity.OwnsMany(a => a.RecentComments, comment => 
                {
                    comment.Property(c => c.Id).HasElementName("id").HasMaxLength(36);
                    comment.Property(c => c.AuthorId).HasElementName("author_id").HasMaxLength(36);
                    comment.Property(c => c.AuthorName).HasElementName("author_name").HasMaxLength(64);
                    comment.Property(c => c.AuthorProfilePictureUri).HasElementName("author_profile_picture_uri").HasMaxLength(256);
                    comment.Property(c => c.Content).HasElementName("content").HasMaxLength(1000);
                    comment.Property(c => c.PostedAt).HasElementName("posted_at");
                });
            });

            modelBuilder.Entity<Category>(entity =>
            {
                entity.ToCollection("categories");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Id).HasElementName("_id").HasMaxLength(36);
                entity.Property(c => c.Name).HasElementName("name").HasMaxLength(64).IsRequired();
            });

            modelBuilder.Entity<Comment>(entity =>
            {
                entity.ToCollection("comments");
                entity.HasKey(c => c.Id);
                entity.Property(c => c.Id).HasElementName("_id").HasMaxLength(36);
                
                entity.Property(c => c.ArticleId).HasElementName("article_id").HasMaxLength(36).IsRequired();
                entity.Property(c => c.AuthorId).HasElementName("author_id").HasMaxLength(36).IsRequired();
                entity.Property(c => c.AuthorName).HasElementName("author_name").HasMaxLength(128).IsRequired();
                entity.Property(c => c.AuthorProfilePictureUri).HasElementName("author_profile_picture_uri").HasMaxLength(256);
                entity.Property(c => c.Content).HasElementName("content").HasMaxLength(1000).IsRequired();
                entity.Property(c => c.PostedAt).HasElementName("posted_at").IsRequired();
            });
        }
}
