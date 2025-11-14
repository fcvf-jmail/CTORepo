namespace WebApi.Application.DTOs;

/// <summary>
/// DTO ответа со статьей
/// </summary>
public class ArticleResponse
{
    /// <summary>
    /// Уникальный идентификатор статьи
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Заголовок статьи
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Содержимое статьи
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Идентификатор раздела
    /// </summary>
    public Guid SectionId { get; set; }

    /// <summary>
    /// Список тегов
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Дата создания статьи
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата последнего обновления статьи
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
