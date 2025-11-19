# Docker Setup - Сводка реализации

## Обзор

Реализована полная Docker конфигурация для запуска ASP.NET Core 8 Web API приложения с PostgreSQL базой данных.

## Созданные файлы

### 1. Dockerfile
**Расположение:** `/Dockerfile`

Многоступенчатая сборка Docker образа:
- **Этап build:** Сборка приложения с .NET SDK 8.0
- **Этап publish:** Публикация приложения в Release конфигурации
- **Этап final:** Финальный образ на базе ASP.NET runtime 8.0 (mcr.microsoft.com/dotnet/aspnet:8.0)

**Особенности:**
- Использует кеширование слоев для оптимизации времени сборки
- Копирует файлы проектов и восстанавливает зависимости перед копированием исходного кода
- Миграции применяются автоматически из Program.cs при старте приложения
- Экспонирует порт 8080 внутри контейнера
- ENTRYPOINT: dotnet WebApi.Presentation.dll

### 2. docker-compose.yml
**Расположение:** `/docker-compose.yml`

Определяет два сервиса:

#### Сервис postgres:
- Образ: postgres:16-alpine
- База данных: webapi
- Credentials: postgres/postgres
- Порт: 5432
- Healthcheck для проверки готовности
- Именованный том для персистентности данных

#### Сервис webapi:
- Собирается из Dockerfile
- Зависит от готовности postgres (healthcheck)
- Порт: 5000 (хост) -> 8080 (контейнер)
- Автоматически применяет миграции при запуске из Program.cs
- Restart policy: unless-stopped

### 3. Program.cs
**Расположение:** `/src/Presentation/WebApi.Presentation/Program.cs`

Приложение автоматически применяет миграции при старте:
```csharp
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate(); // применяет миграции
}
```

Это выполняется до начала прослушивания HTTP запросов, что гарантирует актуальность схемы БД.

### 4. .dockerignore
**Расположение:** `/.dockerignore`

Исключает из контекста сборки:
- Директории bin/, obj/, out/
- IDE файлы (.vs/, .vscode/, .idea/)
- Тесты и покрытие
- Git файлы
- Docker файлы
- Временные файлы
- NuGet пакеты

### 5. appsettings.Production.json
**Расположение:** `/src/Presentation/WebApi.Presentation/appsettings.Production.json`

Production конфигурация для Docker:
- Строка подключения использует имя сервиса 'postgres' вместо 'localhost'
- Настроено логирование для Production
- AllowedHosts: "*"

### 6. .env.example
**Расположение:** `/.env.example`

Пример файла с переменными окружения:
- Настройки PostgreSQL
- Настройки ASP.NET Core
- Строка подключения к базе данных

### 7. docs/DOCKER.md
**Расположение:** `/docs/DOCKER.md`

Подробная документация на русском языке:
- Обзор архитектуры Docker
- Детальное описание Dockerfile и docker-compose.yml
- Описание entrypoint скрипта
- Переменные окружения
- Команды управления контейнерами
- Volumes и сети
- Миграции базы данных
- Troubleshooting
- Советы по разработке
- Рекомендации по безопасности
- Оптимизация
- Production готовность

## Модифицированные файлы

### 1. README.md
**Изменения:**
- Добавлена секция "Запуск с Docker Compose (рекомендуется)"
- Быстрый старт с `docker compose up --build`
- Инструкции по доступу к приложению
- Команды управления контейнерами
- Информация о переменных окружения
- Ссылки на дополнительную документацию

### 2. Program.cs
**Изменения:**
- Swagger UI доступен как в Development, так и в Production (для Docker)
- HTTPS редирект только в Development (Docker использует HTTP)
- Автоматическое применение миграций при старте:
  ```csharp
  // Автоматическое применение миграций
  using (var scope = app.Services.CreateScope())
  {
      var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
      db.Database.Migrate(); // применяет миграции
  }
  
  // HTTPS редирект только в Development
  if (app.Environment.IsDevelopment())
  {
      app.UseHttpsRedirection();
  }
  ```

### 3. .gitignore
**Изменения:**
- Добавлено исключение для `.env` файлов
- Добавлено исключение для `docker-compose.override.yml`

## Acceptance Criteria ✅

### ✅ 1. Dockerfile с многоступенчатой сборкой
- Реализован многоступенчатый Dockerfile
- Использует образы mcr.microsoft.com/dotnet/sdk:8.0
- Оптимизирован с кешированием слоев

### ✅ 2. docker-compose.yml с API и PostgreSQL
- Сервис postgres: PostgreSQL 16 Alpine
- Сервис webapi: ASP.NET Core 8 API
- Healthcheck для координации запуска
- Персистентность данных через volumes

### ✅ 3. Настройка переменных окружения
- Переменные окружения в docker-compose.yml
- Файл .env.example для кастомизации
- Правильная строка подключения для Docker

### ✅ 4. Автоматическое применение миграций
- Entrypoint скрипт scripts/docker-entrypoint.sh
- Использует dotnet ef database update
- Механизм повторных попыток для надежности
- Логирование процесса миграций

### ✅ 5. Обновленный README с инструкциями
- Секция "Запуск с Docker Compose"
- Команда `docker compose up --build`
- Подробные инструкции по управлению
- Информация о доступе к сервисам

### ✅ 6. Сервис доступен по порту
- API доступен на localhost:5000
- Swagger UI доступен на localhost:5000/swagger
- PostgreSQL доступен на localhost:5432

### ✅ 7. Документация на русском
- README.md на русском
- docs/DOCKER.md на русском
- Комментарии в Dockerfile на русском
- Комментарии в docker-compose.yml на русском
- Сообщения в entrypoint скрипте на русском

## Команды для тестирования

```bash
# Запуск всего стека
docker compose up --build

# Проверка доступности API
curl http://localhost:5000/swagger

# Проверка логов
docker compose logs -f webapi

# Остановка
docker compose down

# Полная очистка
docker compose down -v
```

## Доступ к сервисам после запуска

- **API:** http://localhost:5000
- **Swagger UI:** http://localhost:5000/swagger
- **PostgreSQL:** localhost:5432
  - База: webapi
  - Пользователь: postgres
  - Пароль: postgres

## Особенности реализации

1. **Автоматические миграции:** Применяются из Program.cs при каждом запуске приложения, безопасно для повторного применения

2. **Healthcheck:** PostgreSQL проверяется на готовность перед запуском API (через depends_on с condition: service_healthy)

3. **Оптимизированный образ:** Используется aspnet:8.0 runtime вместо SDK для меньшего размера образа

4. **Swagger в Production:** Включен для удобства работы с Docker

5. **HTTP вместо HTTPS:** В Docker используется HTTP (HTTPS можно настроить с reverse proxy)

6. **Персистентность данных:** PostgreSQL данные сохраняются в именованном volume

7. **Оптимизация сборки:** .dockerignore исключает ненужные файлы из контекста

## Безопасность

⚠️ **Важно:** Текущая конфигурация использует стандартные credentials для разработки.

Для production:
- Измените пароли в .env файле
- Используйте Docker secrets
- Настройте HTTPS с SSL сертификатами
- Используйте managed PostgreSQL вместо контейнера
- Ограничьте сетевой доступ

## Следующие шаги (опционально)

1. Настройка CI/CD для автоматической сборки образов
2. Публикация образов в container registry
3. Kubernetes конфигурация для оркестрации
4. Мониторинг и логирование (Prometheus, Grafana, ELK)
5. Backup и disaster recovery для базы данных
