# WebApi - ASP.NET Core 8 Web API

## Описание

Проект представляет собой шаблон Web API приложения на базе ASP.NET Core 8 с использованием минимального хостинга (Minimal API).

## Структура решения

Решение организовано по принципам Clean Architecture и разделено на следующие слои:

### Domain (Домен)
`src/Domain/WebApi.Domain/`

Содержит основные бизнес-сущности и интерфейсы домена. Этот слой не имеет зависимостей от других проектов и представляет собой ядро приложения.

**Содержит:**
- `Entities/` - базовые сущности домена
- `Interfaces/` - интерфейсы репозиториев и сервисов

### Application (Приложение)
`src/Application/WebApi.Application/`

Содержит бизнес-логику приложения, обработчики команд и запросов,DTO и маппинг.

**Зависимости:** Domain

**Содержит:**
- `Common/` - общие классы и утилиты
- `Interfaces/` - интерфейсы для работы с инфраструктурой

### Infrastructure (Инфраструктура)
`src/Infrastructure/WebApi.Infrastructure/`

Содержит реализацию работы с базой данных, внешними сервисами и другой инфраструктуры.

**Зависимости:** Domain, Application

**Содержит:**
- `Data/` - контексты базы данных
- `Repositories/` - реализации репозиториев

### Presentation (Представление)
`src/Presentation/WebApi.Presentation/`

Web API слой, содержит эндпоинты, контроллеры и настройку приложения.

**Зависимости:** Application, Infrastructure

## Технологии

- .NET 8.0
- ASP.NET Core 8 Minimal API
- Swagger/OpenAPI
- Nullable Reference Types
- Implicit Usings

## Требования

- .NET 8.0 SDK или выше

## Сборка и запуск

### Сборка решения

```bash
dotnet build
```

### Запуск приложения

```bash
dotnet run --project src/Presentation/WebApi.Presentation/WebApi.Presentation.csproj
```

### Запуск с горячей перезагрузкой

```bash
dotnet watch --project src/Presentation/WebApi.Presentation/WebApi.Presentation.csproj
```

## Документация API

После запуска приложения документация API доступна по адресу:
- Swagger UI: `https://localhost:<port>/swagger`

## Конфигурация

Настройки приложения находятся в файлах:
- `appsettings.json` - общие настройки
- `appsettings.Development.json` - настройки для разработки

## Стиль кода

Проект использует стандартные соглашения по именованию и оформлению кода .NET, определенные в файле `.editorconfig`.

### Основные правила:
- Включены nullable reference types
- Используются implicit usings
- Отступы: 4 пробела
- Комментарии и документация на русском языке
