using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Application.DTOs;
using WebApi.Application.Interfaces;

namespace WebApi.Presentation.Controllers;

/// <summary>
/// Контроллер для работы со статьями
/// </summary>
/// <remarks>
/// Конструктор контроллера статей
/// </remarks>
/// <param name="articleService">Сервис для работы со статьями</param>
[ApiController]
[Route("api/[controller]")]
public class ArticlesController(IArticleService articleService) : ControllerBase
{
    private readonly IArticleService _articleService = articleService;

    /// <summary>
    /// Получить статью по идентификатору
    /// </summary>
    /// <param name="id">Идентификатор статьи</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Статья с указанным идентификатором</returns>
    /// <remarks>
    /// Возвращает статью с указанным идентификатором. Теги в ответе отсортированы по алфавиту.
    /// </remarks>
    /// <response code="200">Статья успешно найдена</response>
    /// <response code="404">Статья не найдена</response>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ArticleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var result = await _articleService.GetByIdAsync(id, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Создать новую статью
    /// </summary>
    /// <param name="request">Данные для создания статьи</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Созданная статья</returns>
    /// <remarks>
    /// Создает новую статью. Раздел определяется автоматически на основе набора тегов. 
    /// Теги дедуплицируются и сортируются по алфавиту.
    /// </remarks>
    /// <response code="201">Статья успешно создана</response>
    /// <response code="400">Неверные данные запроса</response>
    [HttpPost]
    [ProducesResponseType(typeof(ArticleResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Create([FromBody] CreateArticleRequest request, CancellationToken cancellationToken)
    {
        var result = await _articleService.CreateAsync(request, cancellationToken);

        if (!result.IsSuccess)
        {
            return BadRequest(new { error = result.Error });
        }

        return CreatedAtAction(nameof(GetById), new { id = result.Value!.Id }, result.Value);
    }

    /// <summary>
    /// Обновить существующую статью
    /// </summary>
    /// <param name="id">Идентификатор статьи</param>
    /// <param name="request">Данные для обновления статьи</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Обновленная статья</returns>
    /// <remarks>
    /// Обновляет существующую статью. При изменении тегов статья может переместиться в другой раздел
    /// </remarks>
    /// <response code="200">Статья успешно обновлена</response>
    /// <response code="400">Неверные данные запроса</response>
    /// <response code="404">Статья не найдена</response>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ArticleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateArticleRequest request, CancellationToken cancellationToken)
    {
        var result = await _articleService.UpdateAsync(id, request, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Удалить статью
    /// </summary>
    /// <param name="id">Идентификатор статьи</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Результат удаления</returns>
    /// <remarks>
    /// Удаляет статью по указанному идентификатору
    /// </remarks>
    /// <response code="204">Статья успешно удалена</response>
    /// <response code="404">Статья не найдена</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        var result = await _articleService.DeleteAsync(id, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return NoContent();
    }
}
