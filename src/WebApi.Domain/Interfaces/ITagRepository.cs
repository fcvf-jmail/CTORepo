using WebApi.Domain.Entities;

namespace WebApi.Domain.Interfaces;

/// <summary>
/// Репозиторий для работы с тегами
/// </summary>
public interface ITagRepository
{
    /// <summary>
    /// Получить тег по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор тега</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Тег или null, если не найден</returns>
    Task<Tag?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить тег по нормализованному имени (без учета регистра)
    /// </summary>
    /// <param name="normalizedName">Нормализованное имя тега</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Тег или null, если не найден</returns>
    Task<Tag?> GetByNormalizedNameAsync(string normalizedName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить теги по списку нормализованных имен
    /// </summary>
    /// <param name="normalizedNames">Список нормализованных имен тегов</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список найденных тегов</returns>
    Task<List<Tag>> GetByNormalizedNamesAsync(List<string> normalizedNames, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создать новый тег
    /// </summary>
    /// <param name="tag">Создаваемый тег</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task CreateAsync(Tag tag, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить существующий тег
    /// </summary>
    /// <param name="tag">Обновляемый тег</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task UpdateAsync(Tag tag, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить тег по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор тега</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Проверить существование тега по имени (без учета регистра)
    /// </summary>
    /// <param name="name">Имя тега</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>True, если тег существует, иначе false</returns>
    Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Сохранить изменения в базе данных
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}
