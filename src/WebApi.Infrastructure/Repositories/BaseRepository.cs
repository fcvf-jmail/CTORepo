using WebApi.Domain.Entities;
using WebApi.Domain.Interfaces;

namespace WebApi.Infrastructure.Repositories;

/// <summary>
/// Базовая реализация репозитория
/// </summary>
/// <typeparam name="T">Тип сущности</typeparam>
public class BaseRepository<T> : IRepository<T> where T : BaseEntity
{
    private readonly List<T> _entities = new();

    /// <summary>
    /// Получить сущность по идентификатору
    /// </summary>
    public Task<T?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var entity = _entities.FirstOrDefault(e => e.Id == id);
        return Task.FromResult(entity);
    }

    /// <summary>
    /// Получить все сущности
    /// </summary>
    public Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IEnumerable<T>>(_entities);
    }

    /// <summary>
    /// Добавить новую сущность
    /// </summary>
    public Task AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        entity.CreatedAt = DateTime.UtcNow;
        _entities.Add(entity);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Обновить существующую сущность
    /// </summary>
    public Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        return Task.CompletedTask;
    }

    /// <summary>
    /// Удалить сущность
    /// </summary>
    public Task DeleteAsync(T entity, CancellationToken cancellationToken = default)
    {
        _entities.Remove(entity);
        return Task.CompletedTask;
    }
}
