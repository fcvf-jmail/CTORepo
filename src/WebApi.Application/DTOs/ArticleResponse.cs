namespace WebApi.Application.DTOs;

/// <summary>
/// DTO ответа со статьей
/// </summary>
/// <remarks>
/// Теги в ответе всегда отсортированы по алфавиту без учета регистра.
/// Все даты представлены в формате UTC.
/// </remarks>
public class ArticleResponse
{
    /// <summary>
    /// Уникальный идентификатор статьи
    /// </summary>
    /// <example>3fa85f64-5717-4562-b3fc-2c963f66afa6</example>
    public Guid Id { get; set; }

    /// <summary>
    /// Заголовок статьи
    /// </summary>
    /// <example>Введение в Clean Architecture</example>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Содержимое статьи
    /// </summary>
    /// <example>Clean Architecture - это архитектурный подход, предложенный Робертом Мартином...</example>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Идентификатор раздела, к которому принадлежит статья
    /// </summary>
    /// <example>7fa85f64-5717-4562-b3fc-2c963f66afa9</example>
    public Guid SectionId { get; set; }

    /// <summary>
    /// Список тегов статьи (отсортирован по алфавиту)
    /// </summary>
    /// <example>["Архитектура", "Паттерны", "Разработка"]</example>
    public List<string> Tags { get; set; } = [];

    /// <summary>
    /// Дата создания статьи в формате UTC
    /// </summary>
    /// <example>2024-01-15T10:30:00Z</example>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата последнего обновления статьи в формате UTC
    /// </summary>
    /// <example>2024-01-16T14:45:00Z</example>
    public DateTime? UpdatedAt { get; set; }
}
