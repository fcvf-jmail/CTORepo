using Microsoft.EntityFrameworkCore;
using WebApi.Domain.Entities;
using WebApi.Infrastructure.Data.Configurations;

namespace WebApi.Infrastructure.Data;

/// <summary>
/// Контекст базы данных приложения
/// </summary>
public class ApplicationDbContext : DbContext
{
    /// <summary>
    /// Конструктор контекста базы данных
    /// </summary>
    /// <param name="options">Параметры конфигурации контекста</param>
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Набор данных для сущности Tag
    /// </summary>
    public DbSet<Tag> Tags { get; set; } = null!;

    /// <summary>
    /// Набор данных для сущности Article
    /// </summary>
    public DbSet<Article> Articles { get; set; } = null!;

    /// <summary>
    /// Набор данных для сущности Section
    /// </summary>
    public DbSet<Section> Sections { get; set; } = null!;

    /// <summary>
    /// Конфигурирование модели данных с использованием Fluent API
    /// </summary>
    /// <param name="modelBuilder">Построитель модели</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Применение конфигураций сущностей
        modelBuilder.ApplyConfiguration(new TagConfiguration());
        modelBuilder.ApplyConfiguration(new ArticleConfiguration());
        modelBuilder.ApplyConfiguration(new SectionConfiguration());
    }

    /// <summary>
    /// Переопределение метода SaveChanges для автоматического обновления временных меток и нормализации данных
    /// </summary>
    /// <returns>Количество затронутых записей</returns>
    public override int SaveChanges()
    {
        ProcessEntitiesBeforeSave();
        return base.SaveChanges();
    }

    /// <summary>
    /// Переопределение метода SaveChangesAsync для автоматического обновления временных меток и нормализации данных
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Количество затронутых записей</returns>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        ProcessEntitiesBeforeSave();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Обработка сущностей перед сохранением: обновление временных меток и нормализация данных
    /// </summary>
    private void ProcessEntitiesBeforeSave()
    {
        UpdateTimestamps();
        NormalizeTagNames();
    }

    /// <summary>
    /// Обновление временных меток для сущностей при сохранении
    /// </summary>
    private void UpdateTimestamps()
    {
        var entries = ChangeTracker.Entries<BaseEntity>();

        foreach (var entry in entries)
        {
            switch (entry.State)
            {
                case EntityState.Added:
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.UpdatedAt = null;
                    break;
                case EntityState.Modified:
                    entry.Entity.UpdatedAt = DateTime.UtcNow;
                    break;
            }
        }
    }

    /// <summary>
    /// Нормализация названий тегов для обеспечения уникальности без учета регистра
    /// </summary>
    private void NormalizeTagNames()
    {
        var tagEntries = ChangeTracker.Entries<Tag>();

        foreach (var entry in tagEntries)
        {
            if (entry.State == EntityState.Added || entry.State == EntityState.Modified)
            {
                var tag = entry.Entity;
                tag.NormalizedName = tag.Name.ToLowerInvariant();
            }
        }
    }
}
