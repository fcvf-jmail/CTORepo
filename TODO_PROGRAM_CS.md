# TODO: Интеграция миграций в Program.cs

## Статус: ⏳ Ожидает создания Program.cs

Docker конфигурация готова. Когда будет создан файл `src/Presentation/WebApi.Presentation/Program.cs`, необходимо добавить автоматическое применение миграций.

## Расположение файла

```
src/Presentation/WebApi.Presentation/Program.cs
```

## Код для добавления

### Минимальная версия

```csharp
using Microsoft.EntityFrameworkCore;
using WebApi.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Регистрация DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Другие сервисы...
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ===== ВАЖНО: Автоматическое применение миграций =====
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}
// ====================================================

// Настройка middleware
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

### Расширенная версия с логированием

```csharp
using Microsoft.EntityFrameworkCore;
using WebApi.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);

// Регистрация DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Другие сервисы...
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// ===== Автоматическое применение миграций с логированием =====
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
        throw; // Приложение не должно запускаться если миграции не применились
    }
}
// =============================================================

// Настройка middleware
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

## Важные моменты

### 1. Расположение кода
Код применения миграций должен быть:
- ✅ **После** `var app = builder.Build();`
- ✅ **До** `app.Run();`

### 2. Using директивы
Убедитесь что добавлены:
```csharp
using Microsoft.EntityFrameworkCore;
using WebApi.Infrastructure.Data;
```

### 3. Строка подключения
Должна быть в `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=webapi;Username=postgres;Password=postgres"
  }
}
```

Для Docker она будет переопределена через переменные окружения в `docker-compose.yml`.

### 4. NuGet пакеты
Убедитесь что проект Presentation имеет ссылку на:
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.11" />
```

И ссылку на проект Infrastructure:
```xml
<ProjectReference Include="..\..\Infrastructure\WebApi.Infrastructure\WebApi.Infrastructure.csproj" />
```

## Проверка работоспособности

### 1. Локальная проверка
```bash
cd src/Presentation/WebApi.Presentation
dotnet run
```

Ожидаемые логи:
```
info: Microsoft.EntityFrameworkCore.Migrations[20402]
      Applying migration '20231119120000_InitialCreate'.
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: http://localhost:5000
```

### 2. Docker проверка
```bash
docker compose up --build
```

Ожидаемые логи в контейнере:
```
webapi-app  | info: Microsoft.EntityFrameworkCore.Migrations[20402]
webapi-app  |       Applying migration '20231119120000_InitialCreate'.
webapi-app  | info: Microsoft.Hosting.Lifetime[14]
webapi-app  |       Now listening on: http://[::]:8080
```

### 3. Проверка базы данных
```bash
docker compose exec postgres psql -U postgres -d webapi

# В psql:
\dt
```

Должны отобразиться таблицы:
- `__EFMigrationsHistory`
- Таблицы из ваших миграций

## Troubleshooting

### Проблема: "Unable to create table __EFMigrationsHistory"

**Причина:** PostgreSQL не готов к подключению

**Решение:** 
- Docker Compose автоматически обработает через healthcheck
- Локально: убедитесь что PostgreSQL запущен

### Проблема: "No DbContext was found"

**Причина:** DbContext не зарегистрирован в DI

**Решение:**
```csharp
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
```

### Проблема: Миграции не применяются

**Причина:** Миграции не созданы

**Решение:**
```bash
dotnet ef migrations add InitialCreate \
  --project src/Infrastructure/WebApi.Infrastructure \
  --startup-project src/Presentation/WebApi.Presentation \
  --context ApplicationDbContext
```

## Дополнительная информация

Подробную документацию смотрите в:
- [docs/PROGRAM_CS_MIGRATIONS.md](docs/PROGRAM_CS_MIGRATIONS.md)
- [docs/DOCKER.md](docs/DOCKER.md)
- [docs/DATABASE.md](docs/DATABASE.md)

## Контрольный список

При создании Program.cs:
- [ ] Файл создан в правильном месте
- [ ] Добавлены using директивы
- [ ] Зарегистрирован DbContext
- [ ] Добавлен код применения миграций
- [ ] Код миграций находится между `builder.Build()` и `app.Run()`
- [ ] Локально протестировано
- [ ] Docker build успешен
- [ ] Docker run успешен
- [ ] Миграции применились
- [ ] API доступен

## Итоги

Docker конфигурация **готова** и ожидает создания Program.cs с кодом миграций.

После добавления кода миграций:
1. Выполнить `docker compose build`
2. Выполнить `docker compose up`
3. Проверить логи: `docker compose logs webapi`
4. Проверить API: `http://localhost:8080/swagger`

Размер образа с aspnet:8.0 будет около **220MB** (вместо 850MB со sdk:8.0).
