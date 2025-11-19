using WebApi.Domain.Entities;

namespace WebApi.Domain.Interfaces;

/// <summary>
/// Репозиторий для работы со статьями
/// </summary>
public interface IArticleRepository
{
    /// <summary>
    /// Получить статью по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор статьи</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Статья с загруженными тегами или null, если не найдена</returns>
    Task<Article?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить все статьи
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция всех статей</returns>
    Task<IEnumerable<Article>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Создать новую статью
    /// </summary>
    /// <param name="article">Создаваемая статья</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task CreateAsync(Article article, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить существующую статью
    /// </summary>
    /// <param name="article">Обновляемая статья</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task UpdateAsync(Article article, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить статью по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор статьи</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Сохранить изменения в базе данных
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
