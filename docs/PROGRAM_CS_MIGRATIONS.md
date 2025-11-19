# Интеграция миграций в Program.cs

## Обзор

Миграции Entity Framework Core применяются автоматически при запуске приложения из `Program.cs`, без необходимости в отдельных shell скриптах.

## Реализация

### Код применения миграций

Добавьте следующий код в `Program.cs` **после** построения приложения (`var app = builder.Build();`) и **перед** запуском приложения (`app.Run();`):

```csharp
// Автоматическое применение миграций при старте
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}
```

### Полный пример Program.cs

```csharp
using Microsoft.EntityFrameworkCore;
using WebApi.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Добавление сервисов
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Автоматическое применение миграций при старте
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

// Конфигурация middleware
app.UseSwagger();
app.UseSwaggerUI();

if (app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

app.UseAuthorization();
app.MapControllers();

app.Run();
```

## Преимущества

### 1. Простота
- Не требуется отдельный entrypoint скрипт
- Миграции интегрированы непосредственно в код приложения
- Легко тестировать и отлаживать

### 2. Надежность
- Миграции применяются до начала обработки HTTP запросов
- Если миграции не применяются, приложение не запустится
- Использует встроенный механизм Entity Framework Core

### 3. Docker оптимизация
- Позволяет использовать `aspnet:8.0` вместо `sdk:8.0` в runtime образе
- Меньший размер Docker образа
- Не нужны дополнительные инструменты CLI в runtime

### 4. Безопасность
- Метод `Migrate()` безопасен для повторного вызова
- Применяет только pending миграции
- Не перезаписывает существующие данные

## Порядок выполнения

1. Приложение запускается
2. Создается scope для получения сервисов
3. Получается экземпляр `ApplicationDbContext`
4. Вызывается `db.Database.Migrate()`
   - Проверяется наличие таблицы `__EFMigrationsHistory`
   - Определяются pending миграции
   - Применяются только новые миграции
5. Scope освобождается (dispose)
6. Запускается Web API (app.Run())

## Логирование

Entity Framework Core автоматически логирует применение миграций. В логах вы увидите:

```
info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (123ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      CREATE TABLE IF NOT EXISTS "__EFMigrationsHistory" ...

info: Microsoft.EntityFrameworkCore.Migrations[20402]
      Applying migration '20231119120000_InitialCreate'.

info: Microsoft.EntityFrameworkCore.Database.Command[20101]
      Executed DbCommand (45ms) [Parameters=[], CommandType='Text', CommandTimeout='30']
      CREATE TABLE "Articles" ...
```

## Обработка ошибок

Если миграции не могут быть применены (например, PostgreSQL недоступен), приложение выбросит исключение и не запустится. Docker Compose с healthcheck и restart policy обеспечит повторные попытки:

```yaml
webapi:
  restart: unless-stopped
  depends_on:
    postgres:
      condition: service_healthy
```

## Альтернативы (не рекомендуется)

### Entrypoint скрипт (старый подход)
```bash
#!/bin/bash
dotnet ef database update
dotnet WebApi.Presentation.dll
```

**Недостатки:**
- Требует SDK образа вместо runtime
- Сложнее отлаживать
- Дополнительная зависимость от shell скриптов

### Ручное применение миграций
```bash
docker compose exec webapi dotnet ef database update
```

**Недостатки:**
- Требует ручного вмешательства
- Не подходит для автоматизации
- Легко забыть применить

## Production готовность

### Рекомендации для Production

1. **Логирование:** Убедитесь, что логи миграций сохраняются
2. **Мониторинг:** Отслеживайте время применения миграций
3. **Резервные копии:** Делайте backup перед применением миграций
4. **Тестирование:** Тестируйте миграции в staging окружении

### Опциональное улучшение

Добавьте try-catch для более детального логирования ошибок:

```csharp
using (var scope = app.Services.CreateScope())
{
    try
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        
        logger.LogInformation("Применение миграций базы данных...");
        db.Database.Migrate();
        logger.LogInformation("Миграции успешно применены");
    }
    catch (Exception ex)
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
        logger.LogError(ex, "Ошибка при применении миграций базы данных");
        throw;
    }
}
```

## Связанная документация

- [docs/DOCKER.md](DOCKER.md) - Документация по Docker и Docker Compose
- [docs/DATABASE.md](DATABASE.md) - Работа с базой данных и миграциями
- [Entity Framework Core Migrations](https://learn.microsoft.com/en-us/ef/core/managing-schemas/migrations/)
