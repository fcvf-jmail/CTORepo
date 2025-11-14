# Сводка по миграциям базы данных

## ✅ Выполненные задачи

### 1. Установлены необходимые пакеты NuGet

- **WebApi.Infrastructure:**
  - `Npgsql.EntityFrameworkCore.PostgreSQL` 8.0.11 - Провайдер PostgreSQL для EF Core
  - `Microsoft.EntityFrameworkCore` 8.0.11 - EF Core
  - `Microsoft.EntityFrameworkCore.Relational` 8.0.11 - Поддержка реляционных БД

- **WebApi.Presentation:**
  - `Microsoft.EntityFrameworkCore.Design` 8.0.11 - Инструменты для создания миграций

### 2. Настроено подключение к базе данных

- **appsettings.json:**
  ```json
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=webapi;Username=postgres;Password=postgres"
  }
  ```

- **appsettings.Development.json:**
  ```json
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=webapi_dev;Username=postgres;Password=postgres"
  }
  ```

- **Program.cs:**
  Зарегистрирован `ApplicationDbContext` с использованием PostgreSQL:
  ```csharp
  builder.Services.AddDbContext<ApplicationDbContext>(options =>
      options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
  ```

### 3. Создана начальная миграция `InitialCreate`

**Миграция расположена в:** `src/Infrastructure/WebApi.Infrastructure/Migrations/`

**Создаваемые таблицы:**

#### Sections (Разделы)
- `Id` (uuid, PK)
- `Name` (varchar(1024), NOT NULL)
- `CreatedAt` (timestamptz, NOT NULL)
- `UpdatedAt` (timestamptz, NULL)

#### Tags (Теги)
- `Id` (uuid, PK)
- `Name` (varchar(256), NOT NULL)
- `NormalizedName` (varchar(256), NOT NULL) - для уникальности без учета регистра
- `CreatedAt` (timestamptz, NOT NULL)
- `UpdatedAt` (timestamptz, NULL)

#### Articles (Статьи)
- `Id` (uuid, PK)
- `Title` (varchar(256), NOT NULL)
- `Content` (text, NOT NULL)
- `SectionId` (uuid, NOT NULL, FK -> Sections.Id)
- `CreatedAt` (timestamptz, NOT NULL)
- `UpdatedAt` (timestamptz, NULL)

#### ArticleTags (Связь статей и тегов)
- `TagId` (uuid, PK, FK -> Tags.Id)
- `ArticleId` (uuid, PK, FK -> Articles.Id)

### 4. Созданные индексы

- `IX_Articles_SectionId` - для оптимизации запросов по разделу
- `IX_ArticleTags_ArticleId` - для оптимизации join операций
- `IX_Tags_NormalizedName_Unique` - **уникальный индекс** для обеспечения уникальности тегов

### 5. Каскадные удаления (CASCADE)

- Удаление раздела → удаляются все статьи в разделе
- Удаление статьи → удаляются все связи со тегами
- Удаление тега → удаляются все связи со статьями

### 6. Проверка миграции

✅ Миграция успешно применена к тестовой базе данных PostgreSQL
✅ Все таблицы созданы корректно
✅ Все индексы и ограничения установлены
✅ Внешние ключи с каскадным поведением работают

## 📝 Команды для работы с миграциями

### Применить миграции к БД:
```bash
dotnet ef database update \
  --project src/Infrastructure/WebApi.Infrastructure/WebApi.Infrastructure.csproj \
  --startup-project src/Presentation/WebApi.Presentation/WebApi.Presentation.csproj \
  --context ApplicationDbContext
```

### Создать новую миграцию:
```bash
dotnet ef migrations add <MigrationName> \
  --project src/Infrastructure/WebApi.Infrastructure/WebApi.Infrastructure.csproj \
  --startup-project src/Presentation/WebApi.Presentation/WebApi.Presentation.csproj \
  --context ApplicationDbContext
```

### Список миграций:
```bash
dotnet ef migrations list \
  --project src/Infrastructure/WebApi.Infrastructure/WebApi.Infrastructure.csproj \
  --startup-project src/Presentation/WebApi.Presentation/WebApi.Presentation.csproj \
  --context ApplicationDbContext
```

## 📚 Документация

Подробная документация по работе с базой данных доступна в файле: **`docs/DATABASE.md`**

## 🚀 Быстрый старт с PostgreSQL

### Запуск PostgreSQL в Docker:
```bash
docker run -d \
  --name postgres-dev \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=webapi \
  -p 5432:5432 \
  postgres:16-alpine
```

### Применение миграций:
```bash
cd /home/engine/project
dotnet ef database update \
  --project src/Infrastructure/WebApi.Infrastructure/WebApi.Infrastructure.csproj \
  --startup-project src/Presentation/WebApi.Presentation/WebApi.Presentation.csproj \
  --context ApplicationDbContext
```

### Проверка созданных таблиц:
```bash
docker exec -it postgres-dev psql -U postgres -d webapi -c "\dt"
```

## ✅ Acceptance Criteria - Выполнено

1. ✅ Миграция `InitialCreate` создана
2. ✅ Создаются таблицы: Tags, Articles, Sections, ArticleTags
3. ✅ Включены ограничения nullability
4. ✅ Установлены ограничения длины (256, 1024 символов)
5. ✅ Уникальный индекс на Tags.NormalizedName
6. ✅ Внешние ключи с каскадным удалением (CASCADE)
7. ✅ Настроена конфигурация подключения (DefaultConnection)
8. ✅ `dotnet ef database update` выполняется успешно

## 🎯 Дата создания миграции

**20251114110718_InitialCreate** - 14 ноября 2025, 11:07:18 UTC
