namespace WebApi.Domain.Entities;

/// <summary>
/// Сущность статьи
/// </summary>
public class Article : BaseEntity
{
    /// <summary>
    /// Заголовок статьи (максимум 256 символов)
    /// </summary>
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Содержимое статьи
    /// </summary>
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Идентификатор раздела, к которому принадлежит статья
    /// </summary>
    public Guid SectionId { get; set; }

    /// <summary>
    /// Раздел, к которому принадлежит статья
    /// </summary>
    public Section Section { get; set; } = null!;

    /// <summary>
    /// Коллекция тегов, связанных со статьей
    /// </summary>
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();
}
