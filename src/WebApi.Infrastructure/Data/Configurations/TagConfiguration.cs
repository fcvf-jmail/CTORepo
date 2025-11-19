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
        // Настройка таблицы
        builder.ToTable("Tags");

        // Настройка первичного ключа
        builder.HasKey(t => t.Id);

        // Настройка свойства Name
        builder.Property(t => t.Name)
            .IsRequired()
            .HasMaxLength(256);

        // Настройка свойства NormalizedName
        builder.Property(t => t.NormalizedName)
            .IsRequired()
            .HasMaxLength(256);

        // Создание уникального индекса для NormalizedName для обеспечения уникальности без учета регистра
        builder.HasIndex(t => t.NormalizedName)
            .IsUnique()
            .HasDatabaseName("IX_Tags_NormalizedName_Unique");

        // Настройка даты создания
        builder.Property(t => t.CreatedAt)
            .IsRequired();

        // Настройка даты обновления
        builder.Property(t => t.UpdatedAt)
            .IsRequired(false);

        // Настройка связи многие-ко-многим с Article
        // Коллекция Articles для тега упорядочена по дате создания статьи
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

        // Упорядочивание коллекции Articles по дате создания при загрузке
        builder.Navigation(t => t.Articles)
            .AutoInclude(false);
    }
}
