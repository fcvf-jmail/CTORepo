using Microsoft.EntityFrameworkCore;
using WebApi.Application.DTOs;
using WebApi.Domain.Entities;
using WebApi.Infrastructure.Data;
using WebApi.Infrastructure.Services;

namespace WebApi.Tests;

/// <summary>
/// Тесты для сервиса работы со статьями
/// </summary>
public class ArticleServiceTests : IDisposable
{
    private readonly ApplicationDbContext _context;
    private readonly ArticleService _articleService;
    private readonly SectionService _sectionService;

    public ArticleServiceTests()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _sectionService = new SectionService(_context);
        _articleService = new ArticleService(_context, _sectionService);
    }

    /// <summary>
    /// Тест создания статьи с тегами
    /// </summary>
    [Fact]
    public async Task CreateAsync_WithTags_ShouldCreateArticleWithDeduplicatedTags()
    {
        var request = new CreateArticleRequest
        {
            Title = "Тестовая статья",
            Content = "Содержимое тестовой статьи",
            Tags = new List<string> { "тег1", "Тег2", "ТЕГ1", "тег2", "тег3" }
        };

        var result = await _articleService.CreateAsync(request);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Тестовая статья", result.Value.Title);
        Assert.Equal("Содержимое тестовой статьи", result.Value.Content);
        Assert.NotEqual(Guid.Empty, result.Value.SectionId);
        Assert.Equal(3, result.Value.Tags.Count);
        Assert.Contains("тег1", result.Value.Tags);
        Assert.Contains("Тег2", result.Value.Tags);
        Assert.Contains("тег3", result.Value.Tags);
    }

    /// <summary>
    /// Тест создания статьи без тегов
    /// </summary>
    [Fact]
    public async Task CreateAsync_WithoutTags_ShouldCreateArticleSuccessfully()
    {
        var request = new CreateArticleRequest
        {
            Title = "Статья без тегов",
            Content = "Содержимое статьи без тегов",
            Tags = new List<string>()
        };

        var result = await _articleService.CreateAsync(request);

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.Equal("Статья без тегов", result.Value.Title);
        Assert.Empty(result.Value.Tags);
    }

    /// <summary>
    /// Тест создания статьи с автоматической установкой даты создания
    /// </summary>
    [Fact]
    public async Task CreateAsync_ShouldSetCreatedAtAutomatically()
    {
        var beforeCreate = DateTime.UtcNow;
        var request = new CreateArticleRequest
        {
            Title = "Тестовая статья",
            Content = "Содержимое",
            Tags = new List<string>()
        };

        var result = await _articleService.CreateAsync(request);
        var afterCreate = DateTime.UtcNow;

        Assert.True(result.IsSuccess);
        Assert.NotNull(result.Value);
        Assert.True(result.Value.CreatedAt >= beforeCreate && result.Value.CreatedAt <= afterCreate);
        Assert.Null(result.Value.UpdatedAt);
    }

    /// <summary>
    /// Тест обновления статьи
    /// </summary>
    [Fact]
    public async Task UpdateAsync_ShouldUpdateArticleAndSetUpdatedAt()
    {
        var createRequest = new CreateArticleRequest
        {
            Title = "Исходная статья",
            Content = "Исходное содержимое",
            Tags = new List<string> { "тег1" }
        };

        var createResult = await _articleService.CreateAsync(createRequest);
        Assert.True(createResult.IsSuccess);
        var articleId = createResult.Value!.Id;

        await Task.Delay(100);

        var updateRequest = new UpdateArticleRequest
        {
            Title = "Обновленная статья",
            Content = "Обновленное содержимое",
            Tags = new List<string> { "тег2", "тег3" }
        };

        var updateResult = await _articleService.UpdateAsync(articleId, updateRequest);

        Assert.True(updateResult.IsSuccess);
        Assert.NotNull(updateResult.Value);
        Assert.Equal("Обновленная статья", updateResult.Value.Title);
        Assert.Equal("Обновленное содержимое", updateResult.Value.Content);
        Assert.Equal(2, updateResult.Value.Tags.Count);
        Assert.Contains("тег2", updateResult.Value.Tags);
        Assert.Contains("тег3", updateResult.Value.Tags);
        Assert.NotNull(updateResult.Value.UpdatedAt);
        Assert.True(updateResult.Value.UpdatedAt > updateResult.Value.CreatedAt);
    }

    /// <summary>
    /// Тест обновления статьи с дедупликацией тегов
    /// </summary>
    [Fact]
    public async Task UpdateAsync_WithDuplicateTags_ShouldDeduplicateCaseInsensitive()
    {
        var createRequest = new CreateArticleRequest
        {
            Title = "Тестовая статья",
            Content = "Содержимое",
            Tags = new List<string> { "тег1" }
        };

        var createResult = await _articleService.CreateAsync(createRequest);
        var articleId = createResult.Value!.Id;

        var updateRequest = new UpdateArticleRequest
        {
            Title = "Обновленная статья",
            Content = "Обновленное содержимое",
            Tags = new List<string> { "новыйТег", "НОВЫЙТЕГ", "другойТег", "ДругойТег" }
        };

        var updateResult = await _articleService.UpdateAsync(articleId, updateRequest);

        Assert.True(updateResult.IsSuccess);
        Assert.Equal(2, updateResult.Value!.Tags.Count);
        Assert.Contains("новыйТег", updateResult.Value.Tags);
        Assert.Contains("другойТег", updateResult.Value.Tags);
    }

    /// <summary>
    /// Тест обновления несуществующей статьи
    /// </summary>
    [Fact]
    public async Task UpdateAsync_WithInvalidArticleId_ShouldReturnFailure()
    {
        var updateRequest = new UpdateArticleRequest
        {
            Title = "Обновленная статья",
            Content = "Содержимое",
            Tags = new List<string>()
        };

        var result = await _articleService.UpdateAsync(Guid.NewGuid(), updateRequest);

        Assert.False(result.IsSuccess);
        Assert.Equal("Статья не найдена", result.Error);
    }

    /// <summary>
    /// Тест получения статьи по идентификатору
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_WithValidId_ShouldReturnArticle()
    {
        var createRequest = new CreateArticleRequest
        {
            Title = "Тестовая статья",
            Content = "Содержимое",
            Tags = new List<string> { "тег1", "тег2" }
        };

        var createResult = await _articleService.CreateAsync(createRequest);
        var articleId = createResult.Value!.Id;

        var getResult = await _articleService.GetByIdAsync(articleId);

        Assert.True(getResult.IsSuccess);
        Assert.NotNull(getResult.Value);
        Assert.Equal("Тестовая статья", getResult.Value.Title);
        Assert.Equal(2, getResult.Value.Tags.Count);
    }

    /// <summary>
    /// Тест получения несуществующей статьи
    /// </summary>
    [Fact]
    public async Task GetByIdAsync_WithInvalidId_ShouldReturnFailure()
    {
        var result = await _articleService.GetByIdAsync(Guid.NewGuid());

        Assert.False(result.IsSuccess);
        Assert.Equal("Статья не найдена", result.Error);
    }

    /// <summary>
    /// Тест удаления статьи
    /// </summary>
    [Fact]
    public async Task DeleteAsync_WithValidId_ShouldDeleteArticle()
    {
        var createRequest = new CreateArticleRequest
        {
            Title = "Статья для удаления",
            Content = "Содержимое",
            Tags = new List<string>()
        };

        var createResult = await _articleService.CreateAsync(createRequest);
        var articleId = createResult.Value!.Id;

        var deleteResult = await _articleService.DeleteAsync(articleId);

        Assert.True(deleteResult.IsSuccess);

        var getResult = await _articleService.GetByIdAsync(articleId);
        Assert.False(getResult.IsSuccess);
    }

    /// <summary>
    /// Тест удаления несуществующей статьи
    /// </summary>
    [Fact]
    public async Task DeleteAsync_WithInvalidId_ShouldReturnFailure()
    {
        var result = await _articleService.DeleteAsync(Guid.NewGuid());

        Assert.False(result.IsSuccess);
        Assert.Equal("Статья не найдена", result.Error);
    }

    /// <summary>
    /// Тест сохранения порядка тегов пользователя
    /// </summary>
    [Fact]
    public async Task CreateAsync_ShouldReturnSortedTags()
    {
        var request = new CreateArticleRequest
        {
            Title = "Статья с упорядоченными тегами",
            Content = "Содержимое",
            Tags = new List<string> { "третий", "первый", "второй" }
        };

        var result = await _articleService.CreateAsync(request);

        Assert.True(result.IsSuccess);
        Assert.Equal(3, result.Value!.Tags.Count);
        Assert.Equal("второй", result.Value.Tags[0]);
        Assert.Equal("первый", result.Value.Tags[1]);
        Assert.Equal("третий", result.Value.Tags[2]);
    }

    /// <summary>
    /// Тест использования существующих тегов
    /// </summary>
    [Fact]
    public async Task CreateAsync_ShouldReuseExistingTags()
    {
        var firstRequest = new CreateArticleRequest
        {
            Title = "Первая статья",
            Content = "Содержимое",
            Tags = new List<string> { "общийТег" }
        };

        await _articleService.CreateAsync(firstRequest);

        var tagsCountBefore = await _context.Tags.CountAsync();

        var secondRequest = new CreateArticleRequest
        {
            Title = "Вторая статья",
            Content = "Содержимое",
            Tags = new List<string> { "ОбщийТег" }
        };

        var result = await _articleService.CreateAsync(secondRequest);

        var tagsCountAfter = await _context.Tags.CountAsync();

        Assert.True(result.IsSuccess);
        Assert.Equal(tagsCountBefore, tagsCountAfter);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}
