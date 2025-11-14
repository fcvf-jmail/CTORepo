namespace WebApi.Application.DTOs;

/// <summary>
/// DTO ответа с разделом
/// </summary>
/// <remarks>
/// Разделы создаются автоматически на основе уникальных наборов тегов.
/// Теги в разделе всегда отсортированы по алфавиту без учета регистра.
/// Все даты представлены в формате UTC.
/// </remarks>
public class SectionResponse
{
    /// <summary>
    /// Уникальный идентификатор раздела
    /// </summary>
    /// <example>7fa85f64-5717-4562-b3fc-2c963f66afa9</example>
    public Guid Id { get; set; }

    /// <summary>
    /// Название раздела (формируется из тегов через запятую)
    /// </summary>
    /// <example>Архитектура, Паттерны, Разработка</example>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Список тегов раздела (отсортирован по алфавиту)
    /// </summary>
    /// <example>["Архитектура", "Паттерны", "Разработка"]</example>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Количество статей в разделе
    /// </summary>
    /// <example>5</example>
    public int ArticleCount { get; set; }

    /// <summary>
    /// Дата создания раздела в формате UTC
    /// </summary>
    /// <example>2024-01-15T10:30:00Z</example>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата последнего обновления раздела в формате UTC
    /// </summary>
    /// <example>2024-01-20T09:15:00Z</example>
    public DateTime? UpdatedAt { get; set; }
}
