using WebApi.Application.Common;
using WebApi.Application.DTOs;
using WebApi.Application.Interfaces;
using WebApi.Domain.Entities;
using WebApi.Domain.Interfaces;

namespace WebApi.Infrastructure.Services;

/// <summary>
/// Сервис для работы с разделами
/// </summary>
/// <remarks>
/// Конструктор сервиса
/// </remarks>
/// <param name="sectionRepository">Репозиторий для работы с разделами</param>
/// <param name="tagRepository">Репозиторий для работы с тегами</param>
public class SectionService(ISectionRepository sectionRepository, ITagRepository tagRepository) : ISectionService
{
    private readonly ISectionRepository _sectionRepository = sectionRepository;
    private readonly ITagRepository _tagRepository = tagRepository;

    /// <summary>
    /// Получить все разделы с сортировкой по количеству статей (по убыванию)
    /// </summary>
    public async Task<Result<IEnumerable<SectionResponse>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        var sections = await _sectionRepository.GetAllAsync(cancellationToken);

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

        return Result<IEnumerable<SectionResponse>>.Success(response);
    }

    /// <summary>
    /// Получить статьи раздела с сортировкой по дате изменения/создания
    /// </summary>
    public async Task<Result<IEnumerable<ArticleResponse>>> GetArticlesBySectionIdAsync(Guid sectionId, CancellationToken cancellationToken = default)
    {
        var section = await _sectionRepository.GetByIdAsync(sectionId, cancellationToken);

        if (section == null)
        {
            return Result<IEnumerable<ArticleResponse>>.Failure("Раздел не найден");
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

        return Result<IEnumerable<ArticleResponse>>.Success(response);
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

        return uniqueTagNames.OrderBy(t => t.ToLowerInvariant()).ToList();
    }

    /// <summary>
    /// Поиск раздела по набору тегов
    /// </summary>
    private async Task<Section?> FindSectionByTagsAsync(List<string> sortedTagNames, CancellationToken cancellationToken)
    {
        var normalizedTagNames = sortedTagNames.Select(t => t.ToLowerInvariant()).OrderBy(n => n).ToList();
        return await _sectionRepository.GetByTagsAsync(normalizedTagNames, cancellationToken);
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

        await _sectionRepository.CreateAsync(section, cancellationToken);
        await _sectionRepository.SaveChangesAsync(cancellationToken);

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
