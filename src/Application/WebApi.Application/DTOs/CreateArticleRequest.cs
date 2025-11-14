using System.ComponentModel.DataAnnotations;

namespace WebApi.Application.DTOs;

/// <summary>
/// DTO для создания новой статьи
/// </summary>
public class CreateArticleRequest
{
    /// <summary>
    /// Заголовок статьи
    /// </summary>
    [Required(ErrorMessage = "Заголовок обязателен")]
    [StringLength(256, MinimumLength = 1, ErrorMessage = "Длина заголовка должна быть от 1 до 256 символов")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Содержимое статьи
    /// </summary>
    [Required(ErrorMessage = "Содержимое обязательно")]
    [MinLength(1, ErrorMessage = "Содержимое не может быть пустым")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Идентификатор раздела
    /// </summary>
    [Required(ErrorMessage = "Идентификатор раздела обязателен")]
    public Guid SectionId { get; set; }

    /// <summary>
    /// Список тегов
    /// </summary>
    public List<string> Tags { get; set; } = new();
}
