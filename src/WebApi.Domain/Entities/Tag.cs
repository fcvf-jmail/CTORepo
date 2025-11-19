namespace WebApi.Domain.Entities;

/// <summary>
/// Сущность тега для категоризации статей
/// </summary>
public class Tag : BaseEntity
{
    private string _name = string.Empty;

    /// <summary>
    /// Название тега (максимум 256 символов, уникальное без учета регистра)
    /// </summary>
    public string Name 
    { 
        get => _name;
        set
        {
            _name = value;
            NormalizedName = value.ToLowerInvariant();
        }
    }

    /// <summary>
    /// Нормализованное название тега в нижнем регистре для обеспечения уникальности без учета регистра
    /// </summary>
    public string NormalizedName { get; set; } = string.Empty;

    /// <summary>
    /// Коллекция статей, связанных с данным тегом
    /// </summary>
    public ICollection<Article> Articles { get; set; } = new List<Article>();
}
