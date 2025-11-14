using WebApi.Domain.Entities;

namespace WebApi.Infrastructure.Data;

/// <summary>
/// Контекст базы данных приложения
/// </summary>
public class ApplicationDbContext
{
    /// <summary>
    /// Конструктор контекста базы данных
    /// </summary>
    public ApplicationDbContext()
    {
    }

    /// <summary>
    /// Метод для применения базовых настроек сущностей
    /// </summary>
    /// <param name="entity">Сущность для настройки</param>
    protected virtual void ConfigureBaseEntity<T>(T entity) where T : BaseEntity
    {
        if (entity.Id == Guid.Empty)
        {
            entity.Id = Guid.NewGuid();
            entity.CreatedAt = DateTime.UtcNow;
        }
        else
        {
            entity.UpdatedAt = DateTime.UtcNow;
        }
    }
}
