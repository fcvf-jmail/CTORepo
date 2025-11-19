using Microsoft.EntityFrameworkCore;
using WebApi.Application.DTOs;
using WebApi.Domain.Entities;
using WebApi.Infrastructure.Data;
using WebApi.Infrastructure.Repositories;
using WebApi.Infrastructure.Services;

namespace WebApi.Tests;

/// <summary>
/// Интеграционные тесты для сервиса работы с разделами
/// </summary>
public class SectionServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly SectionService _sectionService;
    private readonly ArticleService _articleService;

    public SectionServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        
        var articleRepository = new ArticleRepository(_context);
        var tagRepository = new TagRepository(_context);
        var sectionRepository = new SectionRepository(_context);
        
        _sectionService = new SectionService(sectionRepository, tagRepository);
        _articleService = new ArticleService(articleRepository, tagRepository, _sectionService);
    }

    /// <summary>
    /// Тест автоматического создания раздела на основе тегов статьи
    /// </summary>
    [Fact]
    public async Task CreateArticle_ShouldCreateSectionAutomatically()
    {
        var request = new CreateArticleRequest
        {
            Title = "Тестовая статья",
            Content = "Содержимое",
            Tags = new List<string> { "тег1", "тег2" }
        };

        var result = await _articleService.CreateAsync(request);

        Assert.True(result.IsSuccess);
        Assert.NotEqual(Guid.Empty, result.Value!.SectionId);

        var section = await _context.Sections
            .Include(s => s.Tags)
            .FirstOrDefaultAsync(s => s.Id == result.Value.SectionId);

        Assert.NotNull(section);
        Assert.Equal(2, section.Tags.Count);
        Assert.Equal("тег1, тег2", section.Name);
    }

    /// <summary>
    /// Тест автоматического переноса статьи при изменении тегов
    /// </summary>
    [Fact]
    public async Task UpdateArticle_WithDifferentTags_ShouldMoveToNewSection()
    {
        var createRequest = new CreateArticleRequest
        {
            Title = "Статья",
            Content = "Содержимое",
            Tags = new List<string> { "тег1" }
        };

        var createResult = await _articleService.CreateAsync(createRequest);
        var originalSectionId = createResult.Value!.SectionId;

        var updateRequest = new UpdateArticleRequest
        {
            Title = "Статья",
            Content = "Содержимое",
            Tags = new List<string> { "тег2" }
        };

        var updateResult = await _articleService.UpdateAsync(createResult.Value.Id, updateRequest);

        Assert.True(updateResult.IsSuccess);
        Assert.NotEqual(originalSectionId, updateResult.Value!.SectionId);
    }

    /// <summary>
    /// Тест создания раздела для статьи без тегов
    /// </summary>
    [Fact]
    public async Task CreateArticle_WithoutTags_ShouldCreateDefaultSection()
    {
        var request = new CreateArticleRequest
        {
            Title = "Статья без тегов",
            Content = "Содержимое",
            Tags = new List<string>()
        };

        var result = await _articleService.CreateAsync(request);

        Assert.True(result.IsSuccess);
        
        var section = await _context.Sections
            .Include(s => s.Tags)
            .FirstOrDefaultAsync(s => s.Id == result.Value!.SectionId);

        Assert.NotNull(section);
        Assert.Equal("Без тегов", section.Name);
        Assert.Empty(section.Tags);
    }

    /// <summary>
    /// Тест повторного использования существующего раздела
    /// </summary>
    [Fact]
    public async Task CreateArticle_WithSameTags_ShouldReuseSameSection()
    {
        var request1 = new CreateArticleRequest
        {
            Title = "Первая статья",
            Content = "Содержимое 1",
            Tags = new List<string> { "тег1", "тег2" }
        };

        var result1 = await _articleService.CreateAsync(request1);

        var request2 = new CreateArticleRequest
        {
            Title = "Вторая статья",
            Content = "Содержимое 2",
            Tags = new List<string> { "тег2", "тег1" } // Тот же набор, но в другом порядке
        };

        var result2 = await _articleService.CreateAsync(request2);

        Assert.Equal(result1.Value!.SectionId, result2.Value!.SectionId);
        
        var sectionsCount = await _context.Sections.CountAsync();
        Assert.Equal(1, sectionsCount);
    }

    /// <summary>
    /// Тест игнорирования регистра при сопоставлении тегов
    /// </summary>
    [Fact]
    public async Task CreateArticle_WithDifferentCaseTags_ShouldReuseSameSection()
    {
        var request1 = new CreateArticleRequest
        {
            Title = "Первая статья",
            Content = "Содержимое 1",
            Tags = new List<string> { "ТЕГ1", "тег2" }
        };

        var result1 = await _articleService.CreateAsync(request1);

        var request2 = new CreateArticleRequest
        {
            Title = "Вторая статья",
            Content = "Содержимое 2",
            Tags = new List<string> { "тег1", "ТЕГ2" }
        };

        var result2 = await _articleService.CreateAsync(request2);

        Assert.Equal(result1.Value!.SectionId, result2.Value!.SectionId);
    }

    /// <summary>
    /// Тест получения всех разделов с сортировкой по количеству статей
    /// </summary>
    [Fact]
    public async Task GetAllSections_ShouldReturnSortedByArticleCount()
    {
        await _articleService.CreateAsync(new CreateArticleRequest
        {
            Title = "Статья 1",
            Content = "Содержимое",
            Tags = new List<string> { "тег1" }
        });

        await _articleService.CreateAsync(new CreateArticleRequest
        {
            Title = "Статья 2",
            Content = "Содержимое",
            Tags = new List<string> { "тег1" }
        });

        await _articleService.CreateAsync(new CreateArticleRequest
        {
            Title = "Статья 3",
            Content = "Содержимое",
            Tags = new List<string> { "тег2" }
        });

        var result = await _sectionService.GetAllAsync();

        Assert.True(result.IsSuccess);
        Assert.Equal(2, result.Value!.Count);
        Assert.Equal(2, result.Value[0].ArticleCount);
        Assert.Equal(1, result.Value[1].ArticleCount);
    }

    /// <summary>
    /// Тест получения статей раздела с сортировкой по дате
    /// </summary>
    [Fact]
    public async Task GetArticlesBySection_ShouldReturnSortedByDate()
    {
        var result1 = await _articleService.CreateAsync(new CreateArticleRequest
        {
            Title = "Статья 1",
            Content = "Содержимое",
            Tags = new List<string> { "тег1" }
        });

        await Task.Delay(100);

        var result2 = await _articleService.CreateAsync(new CreateArticleRequest
        {
            Title = "Статья 2",
            Content = "Содержимое",
            Tags = new List<string> { "тег1" }
        });

        await Task.Delay(100);

        await _articleService.UpdateAsync(result1.Value!.Id, new UpdateArticleRequest
        {
            Title = "Статья 1 обновлена",
            Content = "Содержимое",
            Tags = new List<string> { "тег1" }
        });

        var sectionId = result1.Value.SectionId;
        var articlesResult = await _sectionService.GetArticlesBySectionIdAsync(sectionId);

        Assert.True(articlesResult.IsSuccess);
        Assert.Equal(2, articlesResult.Value!.Count);
        Assert.Equal("Статья 1 обновлена", articlesResult.Value[0].Title);
        Assert.Equal("Статья 2", articlesResult.Value[1].Title);
    }

    /// <summary>
    /// Тест получения статей для несуществующего раздела
    /// </summary>
    [Fact]
    public async Task GetArticlesBySection_WithInvalidSectionId_ShouldReturnFailure()
    {
        var result = await _sectionService.GetArticlesBySectionIdAsync(Guid.NewGuid());

        Assert.False(result.IsSuccess);
        Assert.Equal("Раздел не найден", result.Error);
    }

    /// <summary>
    /// Тест возвращения упорядоченных тегов в ответе
    /// </summary>
    [Fact]
    public async Task GetAllSections_ShouldReturnSortedTags()
    {
        await _articleService.CreateAsync(new CreateArticleRequest
        {
            Title = "Статья",
            Content = "Содержимое",
            Tags = new List<string> { "третий", "первый", "второй" }
        });

        var result = await _sectionService.GetAllAsync();

        Assert.True(result.IsSuccess);
        var section = result.Value!.First();
        Assert.Equal(3, section.Tags.Count);
        Assert.Equal("второй", section.Tags[0]);
        Assert.Equal("первый", section.Tags[1]);
        Assert.Equal("третий", section.Tags[2]);
    }

    /// <summary>
    /// Тест обрезки имени раздела до 1024 символов
    /// </summary>
    [Fact]
    public async Task CreateSection_WithLongName_ShouldTruncateTo1024Chars()
    {
        var longTags = new List<string>();
        for (int i = 0; i < 100; i++)
        {
            longTags.Add($"очень_длинный_тег_{i:D3}");
        }

        var result = await _articleService.CreateAsync(new CreateArticleRequest
        {
            Title = "Статья",
            Content = "Содержимое",
            Tags = longTags
        });

        Assert.True(result.IsSuccess);
        
        var section = await _context.Sections.FindAsync(result.Value!.SectionId);
        Assert.NotNull(section);
        Assert.True(section.Name.Length <= 1024);
    }

    /// <summary>
    /// Тест что теги в статьях возвращаются упорядоченными
    /// </summary>
    [Fact]
    public async Task GetArticlesBySection_ShouldReturnArticlesWithSortedTags()
    {
        var result = await _articleService.CreateAsync(new CreateArticleRequest
        {
            Title = "Статья",
            Content = "Содержимое",
            Tags = new List<string> { "z-тег", "a-тег", "m-тег" }
        });

        var sectionId = result.Value!.SectionId;
        var articlesResult = await _sectionService.GetArticlesBySectionIdAsync(sectionId);

        Assert.True(articlesResult.IsSuccess);
        var article = articlesResult.Value!.First();
        Assert.Equal(3, article.Tags.Count);
        Assert.Equal("a-тег", article.Tags[0]);
        Assert.Equal("m-тег", article.Tags[1]);
        Assert.Equal("z-тег", article.Tags[2]);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
