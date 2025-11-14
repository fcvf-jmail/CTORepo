using WebApi.Domain.Entities;

namespace WebApi.Domain.Interfaces;

/// <summary>
/// Базовый интерфейс репозитория для работы с сущностями
/// </summary>
/// <typeparam name="T">Тип сущности, наследующейся от BaseEntity</typeparam>
public interface IRepository<T> where T : BaseEntity
{
    /// <summary>
    /// Получить сущность по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор сущности</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Сущность или null, если не найдена</returns>
    Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить все сущности
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция всех сущностей</returns>
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Добавить новую сущность
    /// </summary>
    /// <param name="entity">Добавляемая сущность</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task AddAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить существующую сущность
    /// </summary>
    /// <param name="entity">Обновляемая сущность</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить сущность
    /// </summary>
    /// <param name="entity">Удаляемая сущность</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task DeleteAsync(T entity, CancellationToken cancellationToken = default);
}
