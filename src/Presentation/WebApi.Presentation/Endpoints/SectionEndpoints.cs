using Microsoft.AspNetCore.Mvc;
using WebApi.Application.DTOs;
using WebApi.Application.Interfaces;

namespace WebApi.Presentation.Endpoints;

/// <summary>
/// Эндпоинты для работы с разделами
/// </summary>
public static class SectionEndpoints
{
    /// <summary>
    /// Регистрация эндпоинтов для разделов
    /// </summary>
    public static void MapSectionEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/sections")
            .WithTags("Sections")
            .WithDescription("Операции для работы с разделами");

        group.MapGet("/", GetAllSections)
            .WithName("GetAllSections")
            .WithSummary("Получить все разделы")
            .WithDescription("Возвращает список всех разделов, отсортированный по количеству статей (убывание). Теги в каждом разделе отсортированы по алфавиту.")
            .WithOpenApi()
            .Produces<List<SectionResponse>>(StatusCodes.Status200OK);

        group.MapGet("/{sectionId}/articles", GetArticlesBySection)
            .WithName("GetArticlesBySection")
            .WithSummary("Получить статьи раздела")
            .WithDescription("Возвращает список статей указанного раздела, отсортированный по дате обновления/создания (убывание). Теги в каждой статье отсортированы по алфавиту.")
            .WithOpenApi()
            .Produces<List<ArticleResponse>>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);
    }

    /// <summary>
    /// Получить все разделы
    /// </summary>
    private static async Task<IResult> GetAllSections(
        [FromServices] ISectionService sectionService,
        CancellationToken cancellationToken)
    {
        var result = await sectionService.GetAllAsync(cancellationToken);

        if (!result.IsSuccess)
        {
            return Results.Problem(result.Error);
        }

        return Results.Ok(result.Value);
    }

    /// <summary>
    /// Получить статьи раздела
    /// </summary>
    private static async Task<IResult> GetArticlesBySection(
        [FromRoute] Guid sectionId,
        [FromServices] ISectionService sectionService,
        CancellationToken cancellationToken)
    {
        var result = await sectionService.GetArticlesBySectionIdAsync(sectionId, cancellationToken);

        if (!result.IsSuccess)
        {
            return Results.NotFound(new { error = result.Error });
        }

        return Results.Ok(result.Value);
    }
}
