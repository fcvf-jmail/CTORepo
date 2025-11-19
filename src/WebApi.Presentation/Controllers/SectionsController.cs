using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using WebApi.Application.DTOs;
using WebApi.Application.Interfaces;

namespace WebApi.Presentation.Controllers;

/// <summary>
/// Контроллер для работы с разделами
/// </summary>
/// <remarks>
/// Конструктор контроллера разделов
/// </remarks>
/// <param name="sectionService">Сервис для работы с разделами</param>
[ApiController]
[Route("api/[controller]")]
public class SectionsController(ISectionService sectionService) : ControllerBase
{
    private readonly ISectionService _sectionService = sectionService;

    /// <summary>
    /// Получить все разделы
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список всех разделов</returns>
    /// <remarks>
    /// Возвращает список всех разделов, отсортированный по количеству статей (убывание)
    /// Теги в каждом разделе отсортированы по алфавиту
    /// </remarks>
    /// <response code="200">Список разделов успешно получен</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<SectionResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var result = await _sectionService.GetAllAsync(cancellationToken);

        if (!result.IsSuccess)
        {
            return Problem(result.Error);
        }

        return Ok(result.Value);
    }

    /// <summary>
    /// Получить статьи раздела
    /// </summary>
    /// <param name="sectionId">Идентификатор раздела</param>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Список статей указанного раздела</returns>
    /// <remarks>
    /// Возвращает список статей указанного раздела, отсортированный по дате обновления/создания (убывание)
    /// Теги в каждой статье отсортированы по алфавиту.
    /// </remarks>
    /// <response code="200">Список статей успешно получен</response>
    /// <response code="404">Раздел не найден</response>
    [HttpGet("{sectionId}/articles")]
    [ProducesResponseType(typeof(List<ArticleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetArticles(Guid sectionId, CancellationToken cancellationToken)
    {
        var result = await _sectionService.GetArticlesBySectionIdAsync(sectionId, cancellationToken);

        if (!result.IsSuccess)
        {
            return NotFound(new { error = result.Error });
        }

        return Ok(result.Value);
    }
}
