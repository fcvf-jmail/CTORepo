# Тесты проекта WebApi

## Обзор

Данная директория содержит unit-тесты для проекта WebApi. Тесты написаны с использованием xUnit и проверяют основную функциональность сервиса работы со статьями.

## Структура тестов

### WebApi.Tests

Проект содержит unit-тесты для сервисного слоя приложения.

**Технологии:**
- xUnit 2.5.3 - фреймворк для тестирования
- EntityFrameworkCore.InMemory 8.0.11 - in-memory база данных для тестирования
- .NET 8.0

**Зависимости:**
- WebApi.Domain
- WebApi.Application
- WebApi.Infrastructure

## Файлы тестов

### ArticleServiceTests.cs

Содержит 13 unit-тестов для проверки работы `ArticleService`:

#### Тесты создания статей
1. `CreateAsync_WithTags_ShouldCreateArticleWithDeduplicatedTags` - проверка создания статьи с дедупликацией тегов
2. `CreateAsync_WithoutTags_ShouldCreateArticleSuccessfully` - создание статьи без тегов
3. `CreateAsync_WithInvalidSectionId_ShouldReturnFailure` - валидация несуществующего раздела
4. `CreateAsync_ShouldSetCreatedAtAutomatically` - автоматическая установка даты создания
5. `CreateAsync_ShouldPreserveUserTagOrder` - сохранение порядка тегов пользователя
6. `CreateAsync_ShouldReuseExistingTags` - переиспользование существующих тегов

#### Тесты обновления статей
7. `UpdateAsync_ShouldUpdateArticleAndSetUpdatedAt` - обновление статьи и установка UpdatedAt
8. `UpdateAsync_WithDuplicateTags_ShouldDeduplicateCaseInsensitive` - дедупликация при обновлении
9. `UpdateAsync_WithInvalidArticleId_ShouldReturnFailure` - валидация несуществующей статьи

#### Тесты получения статей
10. `GetByIdAsync_WithValidId_ShouldReturnArticle` - получение существующей статьи
11. `GetByIdAsync_WithInvalidId_ShouldReturnFailure` - обработка несуществующей статьи

#### Тесты удаления статей
12. `DeleteAsync_WithValidId_ShouldDeleteArticle` - успешное удаление
13. `DeleteAsync_WithInvalidId_ShouldReturnFailure` - удаление несуществующей статьи

## Особенности тестирования

### In-Memory Database

Каждый тест использует отдельную in-memory базу данных для полной изоляции:

```csharp
var options = new DbContextOptionsBuilder<ApplicationDbContext>()
    .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
    .Options;
```

### Cleanup

Класс `ArticleServiceTests` реализует `IDisposable` для очистки ресурсов после каждого теста:

```csharp
public void Dispose()
{
    _context.Database.EnsureDeleted();
    _context.Dispose();
}
```

### Test Data

Каждый тест использует предварительно созданный тестовый раздел:

```csharp
_testSection = new Section
{
    Id = Guid.NewGuid(),
    Name = "Тестовый раздел",
    CreatedAt = DateTime.UtcNow
};
```

## Покрытие тестами

### Покрываемые сценарии

✅ **CRUD операции:**
- Создание статей
- Чтение статей
- Обновление статей
- Удаление статей

✅ **Обработка тегов:**
- Дедупликация (case-insensitive)
- Сохранение порядка пользователя
- Переиспользование существующих тегов
- Создание новых тегов

✅ **Валидация:**
- Проверка существования раздела
- Проверка существования статьи
- Обработка ошибок

✅ **Автоматизация:**
- Установка CreatedAt при создании
- Установка UpdatedAt при обновлении

## Примеры тестов

### Тест дедупликации тегов

```csharp
[Fact]
public async Task CreateAsync_WithTags_ShouldCreateArticleWithDeduplicatedTags()
{
    var request = new CreateArticleRequest
    {
        Title = "Тестовая статья",
        Content = "Содержимое",
        SectionId = _testSection.Id,
        Tags = new List<string> { "тег1", "Тег2", "ТЕГ1", "тег2", "тег3" }
    };

    var result = await _articleService.CreateAsync(request);

    Assert.True(result.IsSuccess);
    Assert.Equal(3, result.Value.Tags.Count);
    Assert.Contains("тег1", result.Value.Tags);
    Assert.Contains("Тег2", result.Value.Tags);
    Assert.Contains("тег3", result.Value.Tags);
}
```

### Тест автоматических временных меток

```csharp
[Fact]
public async Task UpdateAsync_ShouldUpdateArticleAndSetUpdatedAt()
{
    var createRequest = new CreateArticleRequest { /* ... */ };
    var createResult = await _articleService.CreateAsync(createRequest);
    
    await Task.Delay(100);
    
    var updateRequest = new UpdateArticleRequest { /* ... */ };
    var updateResult = await _articleService.UpdateAsync(articleId, updateRequest);

    Assert.NotNull(updateResult.Value.UpdatedAt);
    Assert.True(updateResult.Value.UpdatedAt > updateResult.Value.CreatedAt);
}
```