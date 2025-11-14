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

## Запуск тестов

### Все тесты

```bash
dotnet test
```

### С детальным выводом

```bash
dotnet test --verbosity normal
```

### Конкретный проект

```bash
dotnet test tests/WebApi.Tests/WebApi.Tests.csproj
```

### Без предварительной сборки

```bash
dotnet test --no-build
```

### С фильтрацией по имени

```bash
dotnet test --filter "FullyQualifiedName~CreateAsync"
```

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

## HTTP-тесты

Файл `articles-api.http` содержит примеры HTTP-запросов для ручного тестирования API:

- Создание статьи
- Получение статьи
- Обновление статьи
- Удаление статьи
- Тесты валидации
- Тесты обработки тегов

**Использование (VS Code с расширением REST Client):**
1. Откройте файл `articles-api.http`
2. Нажмите "Send Request" над нужным запросом

## Результаты тестирования

### Успешный прогон

```
Test run for /home/engine/project/tests/WebApi.Tests/bin/Debug/net8.0/WebApi.Tests.dll
VSTest version 17.11.1 (x64)

Starting test execution, please wait...
A total of 1 test files matched the specified pattern.

Passed!  - Failed:     0, Passed:    13, Skipped:     0, Total:    13, Duration: ~250 ms
```

### Детальные результаты

```
Passed WebApi.Tests.ArticleServiceTests.CreateAsync_WithTags_ShouldCreateArticleWithDeduplicatedTags
Passed WebApi.Tests.ArticleServiceTests.CreateAsync_WithoutTags_ShouldCreateArticleSuccessfully
Passed WebApi.Tests.ArticleServiceTests.CreateAsync_WithInvalidSectionId_ShouldReturnFailure
Passed WebApi.Tests.ArticleServiceTests.CreateAsync_ShouldSetCreatedAtAutomatically
Passed WebApi.Tests.ArticleServiceTests.UpdateAsync_ShouldUpdateArticleAndSetUpdatedAt
Passed WebApi.Tests.ArticleServiceTests.UpdateAsync_WithDuplicateTags_ShouldDeduplicateCaseInsensitive
Passed WebApi.Tests.ArticleServiceTests.UpdateAsync_WithInvalidArticleId_ShouldReturnFailure
Passed WebApi.Tests.ArticleServiceTests.GetByIdAsync_WithValidId_ShouldReturnArticle
Passed WebApi.Tests.ArticleServiceTests.GetByIdAsync_WithInvalidId_ShouldReturnFailure
Passed WebApi.Tests.ArticleServiceTests.DeleteAsync_WithValidId_ShouldDeleteArticle
Passed WebApi.Tests.ArticleServiceTests.DeleteAsync_WithInvalidId_ShouldReturnFailure
Passed WebApi.Tests.ArticleServiceTests.CreateAsync_ShouldPreserveUserTagOrder
Passed WebApi.Tests.ArticleServiceTests.CreateAsync_ShouldReuseExistingTags
```

## Continuous Integration

Тесты можно интегрировать в CI/CD пайплайн:

```yaml
# Пример для GitHub Actions
- name: Run tests
  run: dotnet test --no-build --verbosity normal
```

```yaml
# Пример для GitLab CI
test:
  script:
    - dotnet test --no-build --verbosity normal
```

## Добавление новых тестов

### Шаблон теста

```csharp
[Fact]
public async Task MethodName_Scenario_ExpectedBehavior()
{
    // Arrange - подготовка данных
    var request = new CreateArticleRequest { /* ... */ };
    
    // Act - выполнение действия
    var result = await _articleService.CreateAsync(request);
    
    // Assert - проверка результата
    Assert.True(result.IsSuccess);
    Assert.NotNull(result.Value);
}
```

### Naming Convention

Имена тестов следуют шаблону: `MethodName_Scenario_ExpectedBehavior`

Примеры:
- `CreateAsync_WithValidData_ShouldCreateArticle`
- `GetByIdAsync_WithInvalidId_ShouldReturnFailure`
- `UpdateAsync_WithNewTags_ShouldUpdateArticleTags`

## Troubleshooting

### Тесты не запускаются

```bash
# Очистка и пересборка
dotnet clean
dotnet build
dotnet test
```

### Ошибки подключения к базе данных

Тесты используют in-memory базу данных, реальное подключение к PostgreSQL не требуется.

### Тесты падают с timeout

Увеличьте таймаут в настройках xUnit:

```csharp
[Fact(Timeout = 10000)] // 10 секунд
public async Task LongRunningTest() { /* ... */ }
```

## Следующие шаги

Потенциальные улучшения для тестов:

1. **Integration Tests** - тесты для endpoints с TestServer
2. **Performance Tests** - нагрузочное тестирование
3. **Code Coverage** - отчеты о покрытии кода
4. **Mutation Testing** - проверка качества тестов
5. **E2E Tests** - сквозное тестирование через UI

## Дополнительные ресурсы

- [xUnit Documentation](https://xunit.net/)
- [EF Core In-Memory Provider](https://learn.microsoft.com/en-us/ef/core/providers/in-memory/)
- [Testing in ASP.NET Core](https://learn.microsoft.com/en-us/aspnet/core/test/)
