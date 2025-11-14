using WebApi.Application.Common;
using WebApi.Application.DTOs;

namespace WebApi.Application.Interfaces;

/// <summary>
/// Интерфейс сервиса для работы со статьями
/// </summary>
public interface IArticleService
{
    /// <summary>
    /// Получить статью по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор статьи</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с данными статьи или ошибкой</returns>
    Task<Result<ArticleResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Создать новую статью
    /// </summary>
    /// <param name="request">Данные для создания статьи</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с созданной статьей или ошибкой</returns>
    Task<Result<ArticleResponse>> CreateAsync(CreateArticleRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Обновить существующую статью
    /// </summary>
    /// <param name="id">Идентификатор статьи</param>
    /// <param name="request">Данные для обновления статьи</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с обновленной статьей или ошибкой</returns>
    Task<Result<ArticleResponse>> UpdateAsync(Guid id, UpdateArticleRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Удалить статью
    /// </summary>
    /// <param name="id">Идентификатор статьи</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат операции удаления</returns>
    Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
