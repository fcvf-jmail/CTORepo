using System.ComponentModel.DataAnnotations;

namespace WebApi.Application.DTOs;

/// <summary>
/// DTO для создания новой статьи
/// </summary>
/// <remarks>
/// При создании статьи автоматически определяется раздел на основе набора тегов.
/// Если статьи с таким набором тегов еще не существует, создается новый раздел.
/// Дубликаты тегов удаляются без учета регистра и сортируются по алфавиту.
/// </remarks>
public class CreateArticleRequest
{
    /// <summary>
    /// Заголовок статьи
    /// </summary>
    /// <remarks>
    /// Обязательное поле. Длина от 1 до 256 символов.
    /// </remarks>
    /// <example>Введение в Clean Architecture</example>
    [Required(ErrorMessage = "Заголовок обязателен")]
    [StringLength(256, MinimumLength = 1, ErrorMessage = "Длина заголовка должна быть от 1 до 256 символов")]
    public string Title { get; set; } = string.Empty;

    /// <summary>
    /// Содержимое статьи
    /// </summary>
    /// <remarks>
    /// Обязательное поле. Не может быть пустым.
    /// </remarks>
    /// <example>Clean Architecture - это архитектурный подход, предложенный Робертом Мартином...</example>
    [Required(ErrorMessage = "Содержимое обязательно")]
    [MinLength(1, ErrorMessage = "Содержимое не может быть пустым")]
    public string Content { get; set; } = string.Empty;

    /// <summary>
    /// Список тегов
    /// </summary>
    /// <remarks>
    /// Необязательное поле. Дубликаты тегов удаляются без учета регистра
    /// В ответе теги будут отсортированы по алфавиту
    /// </remarks>
    /// <example>["Архитектура", "Разработка", "Паттерны"]</example>
    public List<string> Tags { get; set; } = [];
}
