namespace WebApi.Application.DTOs;

/// <summary>
/// DTO ответа с разделом
/// </summary>
public class SectionResponse
{
    /// <summary>
    /// Уникальный идентификатор раздела
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Название раздела
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Список тегов раздела (упорядоченный)
    /// </summary>
    public List<string> Tags { get; set; } = new();

    /// <summary>
    /// Количество статей в разделе
    /// </summary>
    public int ArticleCount { get; set; }

    /// <summary>
    /// Дата создания раздела
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Дата последнего обновления раздела
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
