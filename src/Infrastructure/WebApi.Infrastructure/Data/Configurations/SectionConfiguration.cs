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
        // Настройка таблицы
        builder.ToTable("Sections");

        // Настройка первичного ключа
        builder.HasKey(s => s.Id);

        // Настройка свойства Name
        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(1024);

        // Настройка даты создания
        builder.Property(s => s.CreatedAt)
            .IsRequired();

        // Настройка даты обновления
        builder.Property(s => s.UpdatedAt)
            .IsRequired(false);

        // Настройка связи один-ко-многим с Article
        builder.HasMany(s => s.Articles)
            .WithOne(a => a.Section)
            .HasForeignKey(a => a.SectionId)
            .OnDelete(DeleteBehavior.Cascade);

        // Упорядочивание коллекции Articles по дате создания при загрузке
        builder.Navigation(s => s.Articles)
            .AutoInclude(false);
    }
}
