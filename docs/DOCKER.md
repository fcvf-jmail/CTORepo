# Docker и Docker Compose

Этот документ содержит подробную информацию о запуске приложения с использованием Docker.

## Обзор

Приложение настроено для запуска в Docker-контейнерах с использованием Docker Compose. Конфигурация включает:

- **Web API** - ASP.NET Core 8 приложение
- **PostgreSQL** - база данных
- **Автоматические миграции** - Entity Framework Core миграции применяются автоматически при запуске

## Быстрый старт

```bash
# Клонировать репозиторий (если еще не клонирован)
git clone <repository-url>
cd <project-directory>

# Запустить приложение
docker compose up --build
```

После успешного запуска приложение будет доступно по адресу:
- API: http://localhost:8080
- Swagger UI: http://localhost:8080/swagger

## Подробная информация

### Архитектура Docker

#### Dockerfile

Используется многоступенчатая сборка (multi-stage build) для оптимизации размера образа:

1. **Этап build** - сборка приложения с использованием SDK образа
2. **Этап publish** - публикация приложения в Release конфигурации
3. **Этап final** - создание финального образа на базе SDK (для миграций)

#### docker-compose.yml

Определяет два сервиса:

1. **postgres** - контейнер PostgreSQL 16
   - Порт: 5432
   - База данных: webapi
   - Credentials: postgres/postgres
   - Healthcheck для проверки готовности

2. **webapi** - контейнер приложения
   - Порт: 8080
   - Зависит от готовности postgres
   - Автоматически применяет миграции при запуске

### Entrypoint Script

Скрипт `scripts/docker-entrypoint.sh` выполняет следующие действия:

1. Ожидает готовности PostgreSQL (начальная задержка 5 секунд)
2. Применяет миграции Entity Framework Core с повторными попытками (до 30 попыток)
3. Запускает приложение

Если миграции не применяются после 30 попыток, контейнер завершается с ошибкой.

### Переменные окружения

#### PostgreSQL (сервис postgres)

- `POSTGRES_DB` - имя базы данных (по умолчанию: webapi)
- `POSTGRES_USER` - имя пользователя (по умолчанию: postgres)
- `POSTGRES_PASSWORD` - пароль (по умолчанию: postgres)

#### Web API (сервис webapi)

- `ASPNETCORE_ENVIRONMENT` - окружение ASP.NET Core (по умолчанию: Production)
- `ASPNETCORE_URLS` - URL для прослушивания (по умолчанию: http://+:8080)
- `ConnectionStrings__DefaultConnection` - строка подключения к PostgreSQL

**Формат строки подключения:**
```
Host=postgres;Port=5432;Database=webapi;Username=postgres;Password=postgres
```

### Управление контейнерами

#### Запуск

```bash
# Запуск с пересборкой образов
docker compose up --build

# Запуск в фоновом режиме
docker compose up -d

# Запуск с пересборкой в фоновом режиме
docker compose up --build -d
```

#### Остановка

```bash
# Остановка контейнеров (сохраняет данные)
docker compose down

# Остановка и удаление томов (удаляет все данные)
docker compose down -v
```

#### Логи

```bash
# Просмотр логов всех сервисов
docker compose logs

# Просмотр логов с отслеживанием
docker compose logs -f

# Просмотр логов конкретного сервиса
docker compose logs webapi
docker compose logs postgres

# Последние 100 строк логов
docker compose logs --tail=100
```

#### Пересборка

```bash
# Пересборка образов без кеша
docker compose build --no-cache

# Пересборка конкретного сервиса
docker compose build webapi
```

#### Выполнение команд

```bash
# Выполнение команды в работающем контейнере
docker compose exec webapi bash

# Подключение к PostgreSQL
docker compose exec postgres psql -U postgres -d webapi
```

### Volumes (Тома)

Docker Compose создает именованный том для хранения данных PostgreSQL:

```yaml
volumes:
  postgres_data:
    driver: local
```

Данные сохраняются между перезапусками контейнеров, если не использовать `docker compose down -v`.

### Сеть

Создается bridge сеть `webapi-network`, в которой оба сервиса могут общаться друг с другом:

- Сервисы обращаются друг к другу по имени (например, `postgres` из `webapi`)
- Порты 5432 (PostgreSQL) и 8080 (Web API) проброшены на хост

### Миграции базы данных

Миграции применяются автоматически при запуске контейнера webapi с помощью entrypoint скрипта.

Команда, выполняемая для применения миграций:

```bash
dotnet ef database update \
  --project src/Infrastructure/WebApi.Infrastructure/WebApi.Infrastructure.csproj \
  --startup-project src/Presentation/WebApi.Presentation/WebApi.Presentation.csproj \
  --context ApplicationDbContext \
  --no-build
```

Если миграции уже применены, команда безопасно завершается без изменений.

### Troubleshooting

#### Проблема: Контейнер webapi не запускается

**Решение:**
1. Проверьте логи: `docker compose logs webapi`
2. Убедитесь, что PostgreSQL запущен: `docker compose ps postgres`
3. Проверьте, что миграции применяются корректно

#### Проблема: Не могу подключиться к API

**Решение:**
1. Проверьте, что контейнеры запущены: `docker compose ps`
2. Проверьте доступность порта 8080: `curl http://localhost:8080/swagger`
3. Проверьте логи: `docker compose logs webapi`

#### Проблема: Ошибка подключения к базе данных

**Решение:**
1. Убедитесь, что PostgreSQL полностью запущен и готов
2. Проверьте строку подключения в docker-compose.yml
3. Проверьте логи PostgreSQL: `docker compose logs postgres`

#### Проблема: Данные теряются при перезапуске

**Решение:**
Не используйте флаг `-v` при остановке:
```bash
docker compose down  # Сохраняет данные
```

Для полной очистки и пересоздания:
```bash
docker compose down -v  # Удаляет все данные
docker compose up --build
```

### Разработка

При разработке с использованием Docker:

1. **Изменения кода** - требуется пересборка образа:
   ```bash
   docker compose up --build
   ```

2. **Изменения в docker-compose.yml** - достаточно перезапуска:
   ```bash
   docker compose down
   docker compose up
   ```

3. **Новые миграции** - создаются локально, затем применяются через Docker:
   ```bash
   # Локально создать миграцию
   dotnet ef migrations add NewMigration \
     --project src/Infrastructure/WebApi.Infrastructure/WebApi.Infrastructure.csproj \
     --startup-project src/Presentation/WebApi.Presentation/WebApi.Presentation.csproj
   
   # Пересобрать и запустить
   docker compose up --build
   ```

### Безопасность

⚠️ **Важно**: Текущая конфигурация использует стандартные credentials для удобства разработки.

Для production окружения:

1. Измените credentials PostgreSQL
2. Используйте secrets или переменные окружения
3. Настройте HTTPS
4. Ограничьте доступ к портам

Пример использования переменных окружения:

```bash
# Создайте .env файл
cat > .env << EOF
POSTGRES_PASSWORD=your_secure_password
DB_USER=your_db_user
DB_NAME=your_db_name
EOF

# Обновите docker-compose.yml для использования переменных из .env
```

### Оптимизация

Для уменьшения времени сборки:

1. `.dockerignore` файл исключает ненужные файлы из контекста сборки
2. Используется кеширование слоев Docker
3. Зависимости восстанавливаются до копирования исходного кода

### Production готовность

Текущая конфигурация подходит для development. Для production рекомендуется:

1. Использовать отдельный образ с aspnet runtime вместо SDK
2. Настроить логирование в внешнее хранилище
3. Использовать managed база данных вместо контейнера
4. Настроить мониторинг и health checks
5. Использовать secrets management
6. Настроить backup и disaster recovery
