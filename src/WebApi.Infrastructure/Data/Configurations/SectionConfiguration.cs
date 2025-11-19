using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApi.Domain.Entities;

namespace WebApi.Infrastructure.Data.Configurations;

/// <summary>
/// Конфигурация сущности Section для Entity Framework Core
/// </summary>
public class SectionConfiguration : IEntityTypeConfiguration<Section>
{
    public void Configure(EntityTypeBuilder<Section> builder)
    {
        builder.ToTable("Sections");

        builder.HasKey(s => s.Id);

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(1024);

        builder.Property(s => s.CreatedAt)
            .IsRequired();

        builder.Property(s => s.UpdatedAt)
            .IsRequired(false);

        builder.HasMany(s => s.Articles)
            .WithOne(a => a.Section)
            .HasForeignKey(a => a.SectionId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(s => s.Tags)
            .WithMany()
            .UsingEntity(
                "SectionTags",
                l => l.HasOne(typeof(Tag)).WithMany().HasForeignKey("TagId").OnDelete(DeleteBehavior.Cascade),
                r => r.HasOne(typeof(Section)).WithMany().HasForeignKey("SectionId").OnDelete(DeleteBehavior.Cascade),
                j =>
                {
                    j.HasKey("SectionId", "TagId");
                    j.ToTable("SectionTags");
                });

        builder.Navigation(s => s.Articles)
            .AutoInclude(false);
    }
}
