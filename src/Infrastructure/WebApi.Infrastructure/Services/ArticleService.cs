using Microsoft.EntityFrameworkCore;
using WebApi.Application.Common;
using WebApi.Application.DTOs;
using WebApi.Application.Interfaces;
using WebApi.Domain.Entities;
using WebApi.Infrastructure.Data;

namespace WebApi.Infrastructure.Services;

/// <summary>
/// Сервис для работы со статьями
/// </summary>
public class ArticleService : IArticleService
{
    private readonly ApplicationDbContext _context;
    private readonly ISectionService _sectionService;

    /// <summary>
    /// Конструктор сервиса
    /// </summary>
    /// <param name="context">Контекст базы данных</param>
    /// <param name="sectionService">Сервис для работы с разделами</param>
    public ArticleService(ApplicationDbContext context, ISectionService sectionService)
    {
        _context = context;
        _sectionService = sectionService;
    }

    /// <summary>
    /// Получить статью по идентификатору
    /// </summary>
    public async Task<Result<ArticleResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var article = await _context.Articles
            .Include(a => a.Tags)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (article == null)
        {
            return Result<ArticleResponse>.Failure("Статья не найдена");
        }

        var response = MapToResponse(article);
        return Result<ArticleResponse>.Success(response);
    }

    /// <summary>
    /// Создать новую статью
    /// </summary>
    public async Task<Result<ArticleResponse>> CreateAsync(CreateArticleRequest request, CancellationToken cancellationToken = default)
    {
        var sectionResult = await _sectionService.GetOrCreateSectionForTagsAsync(request.Tags, cancellationToken);
        if (!sectionResult.IsSuccess)
        {
            return Result<ArticleResponse>.Failure(sectionResult.Error!);
        }

        var article = new Article
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Content = request.Content,
            SectionId = sectionResult.Value
        };

        var tags = await ProcessTagsAsync(request.Tags, cancellationToken);
        article.Tags = tags;

        _context.Articles.Add(article);
        await _context.SaveChangesAsync(cancellationToken);

        await _context.Entry(article).Collection(a => a.Tags).LoadAsync(cancellationToken);

        var response = MapToResponse(article);
        return Result<ArticleResponse>.Success(response);
    }

    /// <summary>
    /// Обновить существующую статью
    /// </summary>
    public async Task<Result<ArticleResponse>> UpdateAsync(Guid id, UpdateArticleRequest request, CancellationToken cancellationToken = default)
    {
        var article = await _context.Articles
            .Include(a => a.Tags)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);

        if (article == null)
        {
            return Result<ArticleResponse>.Failure("Статья не найдена");
        }

        var sectionResult = await _sectionService.GetOrCreateSectionForTagsAsync(request.Tags, cancellationToken);
        if (!sectionResult.IsSuccess)
        {
            return Result<ArticleResponse>.Failure(sectionResult.Error!);
        }

        article.Title = request.Title;
        article.Content = request.Content;
        article.SectionId = sectionResult.Value;

        var tags = await ProcessTagsAsync(request.Tags, cancellationToken);
        article.Tags.Clear();
        article.Tags = tags;

        await _context.SaveChangesAsync(cancellationToken);

        var response = MapToResponse(article);
        return Result<ArticleResponse>.Success(response);
    }

    /// <summary>
    /// Удалить статью
    /// </summary>
    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var article = await _context.Articles.FindAsync(new object[] { id }, cancellationToken);

        if (article == null)
        {
            return Result<bool>.Failure("Статья не найдена");
        }

        _context.Articles.Remove(article);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    /// <summary>
    /// Обработка тегов: дедупликация и case-insensitive обработка
    /// </summary>
    private async Task<List<Tag>> ProcessTagsAsync(List<string> tagNames, CancellationToken cancellationToken)
    {
        if (tagNames == null || tagNames.Count == 0)
        {
            return new List<Tag>();
        }

        var uniqueTagNames = new List<string>();
        var seenNormalized = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        foreach (var tagName in tagNames)
        {
            if (!string.IsNullOrWhiteSpace(tagName))
            {
                var trimmedName = tagName.Trim();
                if (seenNormalized.Add(trimmedName))
                {
                    uniqueTagNames.Add(trimmedName);
                }
            }
        }

        var tags = new List<Tag>();
        foreach (var tagName in uniqueTagNames)
        {
            var normalizedName = tagName.ToLowerInvariant();
            var existingTag = await _context.Tags
                .FirstOrDefaultAsync(t => t.NormalizedName == normalizedName, cancellationToken);

            if (existingTag != null)
            {
                tags.Add(existingTag);
            }
            else
            {
                var newTag = new Tag
                {
                    Id = Guid.NewGuid(),
                    Name = tagName
                };
                _context.Tags.Add(newTag);
                tags.Add(newTag);
            }
        }

        return tags;
    }

    /// <summary>
    /// Преобразование сущности статьи в DTO ответа
    /// </summary>
    private static ArticleResponse MapToResponse(Article article)
    {
        return new ArticleResponse
        {
            Id = article.Id,
            Title = article.Title,
            Content = article.Content,
            SectionId = article.SectionId,
            Tags = article.Tags.Select(t => t.Name).OrderBy(n => n.ToLowerInvariant()).ToList(),
            CreatedAt = article.CreatedAt,
            UpdatedAt = article.UpdatedAt
        };
    }
}
