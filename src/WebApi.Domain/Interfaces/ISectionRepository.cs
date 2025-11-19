using WebApi.Domain.Entities;

namespace WebApi.Domain.Interfaces;

/// <summary>
/// Репозиторий для работы с разделами
/// </summary>
public interface ISectionRepository
{
    /// <summary>
    /// Получить раздел по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор раздела</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Раздел с загруженными тегами и статьями или null, если не найден</returns>
    Task<Section?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить все разделы
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Коллекция всех разделов с загруженными тегами и статьями</returns>
    Task<IEnumerable<Section>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Найти раздел по набору нормализованных имен тегов
    /// </summary>
    /// <param name="normalizedTagNames">Отсортированный список нормализованных имен тегов</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Раздел с точным совпадением набора тегов или null</returns>
    Task<Section?> GetByTagsAsync(List<string> normalizedTagNames, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создать новый раздел
    /// </summary>
    /// <param name="section">Создаваемый раздел</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task CreateAsync(Section section, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить существующий раздел
    /// </summary>
    /// <param name="section">Обновляемый раздел</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task UpdateAsync(Section section, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить раздел по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор раздела</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Сохранить изменения в базе данных
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
