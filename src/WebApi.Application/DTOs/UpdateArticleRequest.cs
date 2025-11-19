using System.ComponentModel.DataAnnotations;

namespace WebApi.Application.DTOs;

/// <summary>
/// DTO для обновления существующей статьи
/// </summary>
/// <remarks>
/// При изменении тегов статья может автоматически переместиться в другой раздел
/// Дубликаты тегов удаляются без учета регистра и сортируются по алфавиту
/// </remarks>
public class UpdateArticleRequest
{
    /// <summary>
    /// Заголовок статьи
    /// </summary>
    /// <remarks>
    /// Обязательное поле. Длина от 1 до 256 символов.
    /// </remarks>
    /// <example>Обновленное введение в Clean Architecture</example>
    [Required(ErrorMessage = "Заголовок обязателен")]
    [StringLength(256, MinimumLength = 1, ErrorMessage = "Длина заголовка должна быть от 1 до 256 символов")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Содержимое статьи
    /// </summary>
    /// <remarks>
    /// Обязательное поле. Не может быть пустым.
    /// </remarks>
    /// <example>Clean Architecture - это архитектурный подход, который помогает разрабатывать масштабируемые приложения...</example>
    [Required(ErrorMessage = "Содержимое обязательно")]
    [MinLength(1, ErrorMessage = "Содержимое не может быть пустым")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Список тегов
    /// </summary>
    /// <remarks>
    /// Необязательное поле. Дубликаты тегов удаляются без учета регистра.
    /// В ответе теги будут отсортированы по алфавиту.
    /// </remarks>
    /// <example>["Архитектура", "Разработка", "Best Practices"]</example>
    public List<string> Tags { get; set; } = [];
}
