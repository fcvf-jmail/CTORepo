using Microsoft.AspNetCore.Mvc;
using WebApi.Application.DTOs;
using WebApi.Application.Interfaces;

namespace WebApi.Presentation.Endpoints;

/// <summary>
/// Эндпоинты для работы со статьями
/// </summary>
public static class ArticleEndpoints
{
    /// <summary>
    /// Регистрация эндпоинтов для статей
    /// </summary>
    public static void MapArticleEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/articles")
            .WithTags("Articles")
            .WithDescription("Операции для работы со статьями");

        group.MapGet("/{id}", GetArticleById)
            .WithName("GetArticleById")
            .WithSummary("Получить статью по идентификатору")
            .WithDescription("Возвращает статью с указанным идентификатором. Теги в ответе отсортированы по алфавиту.")
            .WithOpenApi()
            .Produces<ArticleResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPost("/", CreateArticle)
            .WithName("CreateArticle")
            .WithSummary("Создать новую статью")
            .WithDescription("Создает новую статью. Раздел определяется автоматически на основе набора тегов. Теги дедуплицируются и сортируются по алфавиту.")
            .WithOpenApi()
            .Produces<ArticleResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        group.MapPut("/{id}", UpdateArticle)
            .WithName("UpdateArticle")
            .WithSummary("Обновить существующую статью")
            .WithDescription("Обновляет существующую статью. При изменении тегов статья может переместиться в другой раздел. Теги дедуплицируются и сортируются по алфавиту.")
            .WithOpenApi()
            .Produces<ArticleResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound);

        group.MapDelete("/{id}", DeleteArticle)
            .WithName("DeleteArticle")
            .WithSummary("Удалить статью")
            .WithDescription("Удаляет статью по указанному идентификатору.")
            .WithOpenApi()
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound);
    }

    /// <summary>
    /// Получить статью по идентификатору
    /// </summary>
    private static async Task<IResult> GetArticleById(
        [FromRoute] Guid id,
        [FromServices] IArticleService articleService,
        CancellationToken cancellationToken)
    {
        var result = await articleService.GetByIdAsync(id, cancellationToken);

        if (!result.IsSuccess)
        {
            return Results.NotFound(new { error = result.Error });
        }

        return Results.Ok(result.Value);
    }

    /// <summary>
    /// Создать новую статью
    /// </summary>
    private static async Task<IResult> CreateArticle(
        [FromBody] CreateArticleRequest request,
        [FromServices] IArticleService articleService,
        CancellationToken cancellationToken)
    {
        var result = await articleService.CreateAsync(request, cancellationToken);

        if (!result.IsSuccess)
        {
            return Results.BadRequest(new { error = result.Error });
        }

        return Results.Created($"/api/articles/{result.Value!.Id}", result.Value);
    }

    /// <summary>
    /// Обновить существующую статью
    /// </summary>
    private static async Task<IResult> UpdateArticle(
        [FromRoute] Guid id,
        [FromBody] UpdateArticleRequest request,
        [FromServices] IArticleService articleService,
        CancellationToken cancellationToken)
    {
        var result = await articleService.UpdateAsync(id, request, cancellationToken);

        if (!result.IsSuccess)
        {
            return Results.NotFound(new { error = result.Error });
        }

        return Results.Ok(result.Value);
    }

    /// <summary>
    /// Удалить статью
    /// </summary>
    private static async Task<IResult> DeleteArticle(
        [FromRoute] Guid id,
        [FromServices] IArticleService articleService,
        CancellationToken cancellationToken)
    {
        var result = await articleService.DeleteAsync(id, cancellationToken);

        if (!result.IsSuccess)
        {
            return Results.NotFound(new { error = result.Error });
        }

        return Results.NoContent();
    }
}
