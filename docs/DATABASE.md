# База данных

## Описание

Проект использует PostgreSQL в качестве СУБД и Entity Framework Core для работы с базой данных.

## Структура базы данных

### Таблицы

#### Sections (Разделы)
- `Id` (uuid, PK) - Уникальный идентификатор раздела
- `Name` (varchar(1024), NOT NULL) - Название раздела
- `CreatedAt` (timestamptz, NOT NULL) - Дата создания
- `UpdatedAt` (timestamptz, NULL) - Дата последнего обновления

#### Articles (Статьи)
- `Id` (uuid, PK) - Уникальный идентификатор статьи
- `Title` (varchar(256), NOT NULL) - Заголовок статьи
- `Content` (text, NOT NULL) - Содержимое статьи
- `SectionId` (uuid, NOT NULL, FK -> Sections.Id) - Идентификатор раздела
- `CreatedAt` (timestamptz, NOT NULL) - Дата создания
- `UpdatedAt` (timestamptz, NULL) - Дата последнего обновления

#### Tags (Теги)
- `Id` (uuid, PK) - Уникальный идентификатор тега
- `Name` (varchar(256), NOT NULL) - Название тега
- `NormalizedName` (varchar(256), NOT NULL, UNIQUE) - Нормализованное название (в нижнем регистре)
- `CreatedAt` (timestamptz, NOT NULL) - Дата создания
- `UpdatedAt` (timestamptz, NULL) - Дата последнего обновления

#### ArticleTags (Связь статей и тегов)
- `ArticleId` (uuid, PK, FK -> Articles.Id) - Идентификатор статьи
- `TagId` (uuid, PK, FK -> Tags.Id) - Идентификатор тега

### Индексы

- `IX_Articles_SectionId` - индекс для оптимизации запросов по разделу
- `IX_ArticleTags_ArticleId` - индекс для оптимизации запросов связи статей и тегов
- `IX_Tags_NormalizedName_Unique` - уникальный индекс для обеспечения уникальности тегов без учета регистра

### Каскадное удаление

- При удалении раздела (`Section`) автоматически удаляются все связанные статьи (`Articles`)
- При удалении статьи (`Article`) автоматически удаляются все связи с тегами (`ArticleTags`)
- При удалении тега (`Tag`) автоматически удаляются все связи со статьями (`ArticleTags`)

## Настройка подключения

Строка подключения настраивается в файле `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5432;Database=webapi;Username=postgres;Password=postgres"
  }
}
```

Для локальной разработки можно переопределить настройки в `appsettings.Development.json`.

## Работа с миграциями

### Применение миграций

Для применения всех миграций к базе данных:

```bash
dotnet ef database update \
  --project src/Infrastructure/WebApi.Infrastructure/WebApi.Infrastructure.csproj \
  --startup-project src/Presentation/WebApi.Presentation/WebApi.Presentation.csproj \
  --context ApplicationDbContext
```

### Создание новой миграции

```bash
dotnet ef migrations add <MigrationName> \
  --project src/Infrastructure/WebApi.Infrastructure/WebApi.Infrastructure.csproj \
  --startup-project src/Presentation/WebApi.Presentation/WebApi.Presentation.csproj \
  --context ApplicationDbContext
```

### Удаление последней миграции

```bash
dotnet ef migrations remove \
  --project src/Infrastructure/WebApi.Infrastructure/WebApi.Infrastructure.csproj \
  --startup-project src/Presentation/WebApi.Presentation/WebApi.Presentation.csproj \
  --context ApplicationDbContext
```

### Генерация SQL-скрипта

Для генерации SQL-скрипта всех миграций:

```bash
dotnet ef migrations script \
  --project src/Infrastructure/WebApi.Infrastructure/WebApi.Infrastructure.csproj \
  --startup-project src/Presentation/WebApi.Presentation/WebApi.Presentation.csproj \
  --context ApplicationDbContext \
  --output migrations.sql
```

### Откат миграции

Для отката к конкретной миграции:

```bash
dotnet ef database update <MigrationName> \
  --project src/Infrastructure/WebApi.Infrastructure/WebApi.Infrastructure.csproj \
  --startup-project src/Presentation/WebApi.Presentation/WebApi.Presentation.csproj \
  --context ApplicationDbContext
```

Для отката всех миграций (удаления всех таблиц):

```bash
dotnet ef database update 0 \
  --project src/Infrastructure/WebApi.Infrastructure/WebApi.Infrastructure.csproj \
  --startup-project src/Presentation/WebApi.Presentation/WebApi.Presentation.csproj \
  --context ApplicationDbContext
```

## Запуск PostgreSQL для разработки

### С помощью Docker

```bash
docker run -d \
  --name postgres-dev \
  -e POSTGRES_PASSWORD=postgres \
  -e POSTGRES_DB=webapi \
  -p 5432:5432 \
  postgres:16-alpine
```

Остановка контейнера:

```bash
docker stop postgres-dev && docker rm postgres-dev
```

### С помощью Docker Compose

Создайте файл `docker-compose.yml` в корне проекта:

```yaml
version: '3.8'
services:
  postgres:
    image: postgres:16-alpine
    container_name: webapi-postgres
    environment:
      POSTGRES_USER: postgres
      POSTGRES_PASSWORD: postgres
      POSTGRES_DB: webapi
    ports:
      - "5432:5432"
    volumes:
      - postgres-data:/var/lib/postgresql/data

volumes:
  postgres-data:
```

Запуск:

```bash
docker-compose up -d
```

Остановка:

```bash
docker-compose down
```

## Начальная миграция

Проект содержит начальную миграцию `InitialCreate`, которая создает все необходимые таблицы, индексы и ограничения.

Для применения миграции к новой базе данных выполните:

```bash
dotnet ef database update
```

(с указанием проектов и контекста, как показано выше)
