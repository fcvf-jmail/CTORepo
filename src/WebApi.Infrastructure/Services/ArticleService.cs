using WebApi.Application.Common;
using WebApi.Application.DTOs;
using WebApi.Application.Interfaces;
using WebApi.Domain.Entities;
using WebApi.Domain.Interfaces;

namespace WebApi.Infrastructure.Services;

/// <summary>
/// Сервис для работы со статьями
/// </summary>
/// <remarks>
/// Конструктор сервиса
/// </remarks>
/// <param name="articleRepository">Репозиторий для работы со статьями</param>
/// <param name="tagRepository">Репозиторий для работы с тегами</param>
/// <param name="sectionService">Сервис для работы с разделами</param>
public class ArticleService(IArticleRepository articleRepository, ITagRepository tagRepository, ISectionService sectionService) : IArticleService
{
    private readonly IArticleRepository _articleRepository = articleRepository;
    private readonly ITagRepository _tagRepository = tagRepository;
    private readonly ISectionService _sectionService = sectionService;

    /// <summary>
    /// Получить статью по идентификатору
    /// </summary>
    public async Task<Result<ArticleResponse>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var article = await _articleRepository.GetByIdAsync(id, cancellationToken);

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

        await _articleRepository.CreateAsync(article, cancellationToken);
        await _articleRepository.SaveChangesAsync(cancellationToken);

        var response = MapToResponse(article);
        return Result<ArticleResponse>.Success(response);
    }

    /// <summary>
    /// Обновить существующую статью
    /// </summary>
    public async Task<Result<ArticleResponse>> UpdateAsync(Guid id, UpdateArticleRequest request, CancellationToken cancellationToken = default)
    {
        var article = await _articleRepository.GetByIdAsync(id, cancellationToken);

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

        await _articleRepository.UpdateAsync(article, cancellationToken);
        await _articleRepository.SaveChangesAsync(cancellationToken);

        var response = MapToResponse(article);
        return Result<ArticleResponse>.Success(response);
    }

    /// <summary>
    /// Удалить статью
    /// </summary>
    public async Task<Result<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var article = await _articleRepository.GetByIdAsync(id, cancellationToken);

        if (article == null)
        {
            return Result<bool>.Failure("Статья не найдена");
        }

        await _articleRepository.DeleteAsync(id, cancellationToken);
        await _articleRepository.SaveChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }

    /// <summary>
    /// Обработка тегов: дедупликация и case-insensitive обработка
    /// </summary>
    private async Task<List<Tag>> ProcessTagsAsync(List<string> tagNames, CancellationToken cancellationToken)
    {
        if (tagNames == null || tagNames.Count == 0)
        {
            return [];
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
            var existingTag = await _tagRepository.GetByNormalizedNameAsync(normalizedName, cancellationToken);

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
                await _tagRepository.CreateAsync(newTag, cancellationToken);
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
