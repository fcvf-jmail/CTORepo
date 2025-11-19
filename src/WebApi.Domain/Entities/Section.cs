namespace WebApi.Domain.Entities;

/// <summary>
/// Сущность раздела для организации статей
/// </summary>
public class Section : BaseEntity
{
    /// <summary>
    /// Название раздела (максимум 1024 символа)
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Коллекция тегов, определяющих данный раздел
    /// </summary>
    public ICollection<Tag> Tags { get; set; } = new List<Tag>();

    /// <summary>
    /// Коллекция статей, принадлежащих данному разделу
    /// </summary>
    public ICollection<Article> Articles { get; set; } = [];
}
