using Microsoft.EntityFrameworkCore;
using WebApi.Application.Common;
using WebApi.Application.DTOs;
using WebApi.Application.Interfaces;
using WebApi.Domain.Entities;
using WebApi.Infrastructure.Data;

namespace WebApi.Infrastructure.Services;

/// <summary>
/// Сервис для работы с разделами
/// </summary>
public class SectionService : ISectionService
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Конструктор сервиса
    /// </summary>
    /// <param name="context">Контекст базы данных</param>
    public SectionService(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Получить все разделы с сортировкой по количеству статей (по убыванию)
    /// </summary>
    public async Task<Result<List<SectionResponse>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var sections = await _context.Sections
            .Include(s => s.Tags)
            .Include(s => s.Articles)
            .ToListAsync(cancellationToken);

        var response = sections
            .Select(s => new SectionResponse
            {
                Id = s.Id,
                Name = s.Name,
                Tags = s.Tags.Select(t => t.Name).OrderBy(n => n.ToLowerInvariant()).ToList(),
                ArticleCount = s.Articles.Count,
                CreatedAt = s.CreatedAt,
                UpdatedAt = s.UpdatedAt
            })
            .OrderByDescending(s => s.ArticleCount)
            .ToList();

        return Result<List<SectionResponse>>.Success(response);
    }

    /// <summary>
    /// Получить статьи раздела с сортировкой по дате изменения/создания
    /// </summary>
    public async Task<Result<List<ArticleResponse>>> GetArticlesBySectionIdAsync(Guid sectionId, CancellationToken cancellationToken = default)
    {
        var section = await _context.Sections
            .Include(s => s.Articles)
                .ThenInclude(a => a.Tags)
            .FirstOrDefaultAsync(s => s.Id == sectionId, cancellationToken);

        if (section == null)
        {
            return Result<List<ArticleResponse>>.Failure("Раздел не найден");
        }

        var response = section.Articles
            .OrderByDescending(a => a.UpdatedAt ?? a.CreatedAt)
            .Select(a => new ArticleResponse
            {
                Id = a.Id,
                Title = a.Title,
                Content = a.Content,
                SectionId = a.SectionId,
                Tags = a.Tags.Select(t => t.Name).OrderBy(n => n.ToLowerInvariant()).ToList(),
                CreatedAt = a.CreatedAt,
                UpdatedAt = a.UpdatedAt
            })
            .ToList();

        return Result<List<ArticleResponse>>.Success(response);
    }

    /// <summary>
    /// Получить или создать раздел для указанного набора тегов
    /// </summary>
    public async Task<Result<Guid>> GetOrCreateSectionForTagsAsync(List<string> tagNames, CancellationToken cancellationToken = default)
    {
        var normalizedAndSorted = NormalizeAndSortTags(tagNames);
        
        var section = await FindSectionByTagsAsync(normalizedAndSorted, cancellationToken);
        
        if (section != null)
        {
            return Result<Guid>.Success(section.Id);
        }

        var newSection = await CreateSectionAsync(normalizedAndSorted, cancellationToken);
        return Result<Guid>.Success(newSection.Id);
    }

    /// <summary>
    /// Нормализация и сортировка тегов для сравнения
    /// </summary>
    private static List<string> NormalizeAndSortTags(List<string> tagNames)
    {
        if (tagNames == null || tagNames.Count == 0)
        {
            return new List<string>();
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

        return uniqueTagNames.OrderBy(t => t.ToLowerInvariant()).ToList();
    }

    /// <summary>
    /// Поиск раздела по набору тегов
    /// </summary>
    private async Task<Section?> FindSectionByTagsAsync(List<string> sortedTagNames, CancellationToken cancellationToken)
    {
        if (sortedTagNames.Count == 0)
        {
            return await _context.Sections
                .Include(s => s.Tags)
                .FirstOrDefaultAsync(s => s.Tags.Count == 0, cancellationToken);
        }

        var normalizedTagNames = sortedTagNames.Select(t => t.ToLowerInvariant()).ToList();

        var sections = await _context.Sections
            .Include(s => s.Tags)
            .Where(s => s.Tags.Count == sortedTagNames.Count)
            .ToListAsync(cancellationToken);

        foreach (var section in sections)
        {
            var sectionTagNames = section.Tags
                .Select(t => t.NormalizedName)
                .OrderBy(n => n)
                .ToList();

            if (normalizedTagNames.SequenceEqual(sectionTagNames))
            {
                return section;
            }
        }

        return null;
    }

    /// <summary>
    /// Создание нового раздела для набора тегов
    /// </summary>
    private async Task<Section> CreateSectionAsync(List<string> sortedTagNames, CancellationToken cancellationToken)
    {
        var tags = await GetOrCreateTagsAsync(sortedTagNames, cancellationToken);
        
        var sectionName = GenerateSectionName(tags);

        var section = new Section
        {
            Id = Guid.NewGuid(),
            Name = sectionName,
            Tags = tags
        };

        _context.Sections.Add(section);
        await _context.SaveChangesAsync(cancellationToken);

        return section;
    }

    /// <summary>
    /// Получение или создание тегов
    /// </summary>
    private async Task<List<Tag>> GetOrCreateTagsAsync(List<string> tagNames, CancellationToken cancellationToken)
    {
        var tags = new List<Tag>();

        foreach (var tagName in tagNames)
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
    /// Генерация имени раздела на основе тегов
    /// </summary>
    private static string GenerateSectionName(List<Tag> tags)
    {
        if (tags.Count == 0)
        {
            return "Без тегов";
        }

        var sortedTagNames = tags.Select(t => t.Name).OrderBy(n => n.ToLowerInvariant()).ToList();
        var name = string.Join(", ", sortedTagNames);

        if (name.Length > 1024)
        {
            name = name.Substring(0, 1024);
        }

        return name;
    }
}
