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
        // Настройка таблицы
        builder.ToTable("Articles");

        // Настройка первичного ключа
        builder.HasKey(a => a.Id);

        // Настройка свойства Title
        builder.Property(a => a.Title)
            .IsRequired()
            .HasMaxLength(256);

        // Настройка свойства Content
        builder.Property(a => a.Content)
            .IsRequired();

        // Настройка внешнего ключа SectionId
        builder.Property(a => a.SectionId)
            .IsRequired();

        // Настройка даты создания
        builder.Property(a => a.CreatedAt)
            .IsRequired();

        // Настройка даты обновления
        builder.Property(a => a.UpdatedAt)
            .IsRequired(false);

        // Создание индекса для SectionId для оптимизации запросов
        builder.HasIndex(a => a.SectionId)
            .HasDatabaseName("IX_Articles_SectionId");

        // Настройка связи многие-к-одному с Section
        builder.HasOne(a => a.Section)
            .WithMany(s => s.Articles)
            .HasForeignKey(a => a.SectionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Упорядочивание коллекции Tags по нормализованному имени при загрузке
        builder.Navigation(a => a.Tags)
            .AutoInclude(false);
    }
}
