using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApi.Domain.Entities;

namespace WebApi.Infrastructure.Data.Configurations;

/// <summary>
/// Конфигурация сущности Tag для Entity Framework Core
/// </summary>
public class TagConfiguration : IEntityTypeConfiguration<Tag>
{
    public void Configure(EntityTypeBuilder<Tag> builder)
    {
        builder.ToTable("Tags");

        builder.HasKey(t => t.Id);

        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(t => t.NormalizedName)
            .IsRequired()
            .HasMaxLength(256);

        builder.HasIndex(t => t.NormalizedName)
            .IsUnique()
            .HasDatabaseName("IX_Tags_NormalizedName_Unique");

        builder.Property(t => t.CreatedAt)
            .IsRequired();

        builder.Property(t => t.UpdatedAt)
            .IsRequired(false);

        builder.HasMany(t => t.Articles)
            .WithMany(a => a.Tags)
            .UsingEntity<Dictionary<string, object>>(
                "ArticleTag",
                j => j.HasOne<Article>().WithMany().HasForeignKey("ArticleId").OnDelete(DeleteBehavior.Cascade),
                j => j.HasOne<Tag>().WithMany().HasForeignKey("TagId").OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.ToTable("ArticleTags");
                    j.HasKey("TagId", "ArticleId");
                    j.HasIndex("ArticleId");
                }
            );

        builder.Navigation(t => t.Articles)
            .AutoInclude(false);
    }
}
