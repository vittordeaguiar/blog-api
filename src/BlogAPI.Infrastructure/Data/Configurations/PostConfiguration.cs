using BlogAPI.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BlogAPI.Infrastructure.Data.Configurations;

public class PostConfiguration : IEntityTypeConfiguration<Post>
{
    public void Configure(EntityTypeBuilder<Post> builder)
    {
        builder.ToTable("Posts");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Title)
            .IsRequired()
            .HasMaxLength(200);

        builder.Property(p => p.Content)
            .IsRequired();

        builder.Property(p => p.Slug)
            .IsRequired()
            .HasMaxLength(250);

        builder.HasIndex(p => p.Slug)
            .IsUnique();

        builder.Property(p => p.AuthorId)
            .IsRequired();

        builder.Property(p => p.CreatedAt)
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .IsRequired(false);

        builder.Property(p => p.PublishedAt)
            .IsRequired(false);

        builder.Property(p => p.IsPublished)
            .IsRequired()
            .HasDefaultValue(false);

        // Relationship: Post -> User (Many-to-One)
        builder.HasOne(p => p.Author)
            .WithMany()
            .HasForeignKey(p => p.AuthorId)
            .OnDelete(DeleteBehavior.Restrict);

        // Relationship: Post <-> Category (Many-to-Many)
        builder.HasMany(p => p.Categories)
            .WithMany(c => c.Posts)
            .UsingEntity<Dictionary<string, object>>(
                "PostCategories",
                j => j.HasOne<Category>().WithMany().HasForeignKey("CategoryId"),
                j => j.HasOne<Post>().WithMany().HasForeignKey("PostId"),
                j =>
                {
                    j.ToTable("PostCategories");
                    j.HasKey("PostId", "CategoryId");
                });
    }
}
