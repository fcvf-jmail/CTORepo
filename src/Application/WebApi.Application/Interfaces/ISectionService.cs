using WebApi.Application.Common;
using WebApi.Application.DTOs;

namespace WebApi.Application.Interfaces;

/// <summary>
/// Интерфейс сервиса для работы с разделами
/// </summary>
public interface ISectionService
{
    /// <summary>
    /// Получить все разделы с сортировкой по количеству статей (по убыванию)
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат со списком разделов или ошибкой</returns>
    Task<Result<List<SectionResponse>>> GetAllAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить статьи раздела с сортировкой по дате изменения/создания
    /// </summary>
    /// <param name="sectionId">Идентификатор раздела</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат со списком статей или ошибкой</returns>
    Task<Result<List<ArticleResponse>>> GetArticlesBySectionIdAsync(Guid sectionId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Получить или создать раздел для указанного набора тегов
    /// </summary>
    /// <param name="tagNames">Список названий тегов</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат с идентификатором раздела или ошибкой</returns>
    Task<Result<Guid>> GetOrCreateSectionForTagsAsync(List<string> tagNames, CancellationToken cancellationToken = default);
}
