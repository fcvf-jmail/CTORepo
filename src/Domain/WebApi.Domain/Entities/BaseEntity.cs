namespace WebApi.Domain.Entities;

/// <summary>
/// Базовый класс для всех сущностей домена
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// Уникальный идентификатор сущности
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Дата создания записи
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата последнего обновления записи
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
