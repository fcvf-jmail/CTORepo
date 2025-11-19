using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApi.Domain.Entities;

namespace WebApi.Infrastructure.Data.Configurations;

/// <summary>
/// Конфигурация сущности Article для Entity Framework Core
/// </summary>
public class ArticleConfiguration : IEntityTypeConfiguration<Article>
{
    public void Configure(EntityTypeBuilder<Article> builder)
    {
        builder.ToTable("Articles");

        builder.HasKey(a => a.Id);

        builder.Property(a => a.Title)
            .IsRequired()
            .HasMaxLength(256);

        builder.Property(a => a.Content)
            .IsRequired();

        builder.Property(a => a.SectionId)
            .IsRequired();

        builder.Property(a => a.CreatedAt)
            .IsRequired();

        builder.Property(a => a.UpdatedAt)
            .IsRequired(false);

        builder.HasIndex(a => a.SectionId)
            .HasDatabaseName("IX_Articles_SectionId");

        builder.HasOne(a => a.Section)
            .WithMany(s => s.Articles)
            .HasForeignKey(a => a.SectionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.Navigation(a => a.Tags)
            .AutoInclude(false);
    }
}