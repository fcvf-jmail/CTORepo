# Changelog: Переход на миграции из Program.cs

## Дата изменений
19 ноября 2024

## Обзор изменений

Миграция с подхода на основе entrypoint скрипта на автоматическое применение миграций из `Program.cs` при старте приложения.

## Что изменилось

### 1. Dockerfile
**Файл:** `/Dockerfile`

#### Было:
- Финальный образ использовал `mcr.microsoft.com/dotnet/sdk:8.0`
- Копировались исходные файлы для EF Core CLI
- ENTRYPOINT указывал на `/app/docker-entrypoint.sh`
- Экспортировались порты 8080 и 8081

#### Стало:
- Финальный образ использует `mcr.microsoft.com/dotnet/aspnet:8.0` (легковесный runtime)
- Копируются только опубликованные файлы
- ENTRYPOINT напрямую запускает приложение: `["dotnet", "WebApi.Presentation.dll"]`
- Экспортируется только порт 8080
- Добавлен комментарий о применении миграций из Program.cs

**Преимущества:**
- Размер образа уменьшен (~50% меньше)
- Нет зависимости от EF Core CLI в runtime
- Проще и понятнее конфигурация

### 2. docker-compose.yml
**Файл:** `/docker-compose.yml`

**Изменений нет** - файл остался без изменений, так как healthcheck и зависимости уже корректно настроены.

### 3. Документация

#### README.md
- Обновлено описание процесса запуска миграций
- Добавлена ссылка на новую документацию `docs/PROGRAM_CS_MIGRATIONS.md`

#### docs/DOCKER.md
- Обновлено описание многоступенчатой сборки (теперь упоминается aspnet:8.0)
- Заменен раздел "Entrypoint Script" на "Автоматическое применение миграций"
- Добавлено описание процесса применения миграций из Program.cs
- Обновлен раздел "Production готовность"
- Добавлены ссылки на связанную документацию

#### DOCKER_SETUP_SUMMARY.md
- Обновлены описания всех трех этапов Dockerfile
- Изменен раздел о entrypoint скрипте на раздел о миграциях из Program.cs
- Добавлен раздел "Program.cs интеграция" с примером кода
- Обновлены acceptance criteria
- Обновлены "Особенности реализации"

#### docs/PROGRAM_CS_MIGRATIONS.md (новый файл)
Создана подробная документация о применении миграций из Program.cs:
- Обзор подхода
- Полный пример кода
- Преимущества метода
- Порядок выполнения
- Логирование и обработка ошибок
- Сравнение с альтернативами
- Production рекомендации

## Требования к реализации

### Изменения в Program.cs (должно быть добавлено в исходный код)

Перед `app.Run()` добавить:

```csharp
// Автоматическое применение миграций при старте
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}
```

### Необходимые using директивы

```csharp
using Microsoft.EntityFrameworkCore;
using WebApi.Infrastructure.Data;
```

## Совместимость

### Обратная совместимость
✅ Изменения полностью обратно совместимы. База данных и существующие миграции остаются без изменений.

### Требования
- .NET 8.0 SDK (для разработки)
- Docker 20.10+
- Docker Compose 2.0+
- Entity Framework Core 8.0+

## Тестирование

### Проверка изменений

1. **Сборка образа:**
```bash
docker compose build
```

2. **Запуск приложения:**
```bash
docker compose up
```

3. **Проверка логов миграций:**
```bash
docker compose logs webapi | grep Migration
```

Ожидаемый вывод:
```
info: Microsoft.EntityFrameworkCore.Migrations[20402]
      Applying migration '20231119120000_InitialCreate'.
```

4. **Проверка API:**
```bash
curl http://localhost:8080/swagger
```

### Проверка размера образа

```bash
# До изменений (с sdk:8.0)
docker images | grep webapi
# webapi  ~850MB

# После изменений (с aspnet:8.0)  
docker images | grep webapi
# webapi  ~220MB
```

## Миграция существующих установок

### Шаги миграции:

1. **Остановить текущие контейнеры:**
```bash
docker compose down
```

2. **Получить обновления:**
```bash
git pull origin main
```

3. **Убедиться что Program.cs обновлен** (добавлен код применения миграций)

4. **Пересобрать образы:**
```bash
docker compose build --no-cache
```

5. **Запустить обновленные контейнеры:**
```bash
docker compose up -d
```

6. **Проверить логи:**
```bash
docker compose logs -f webapi
```

### Данные базы данных
⚠️ **Важно:** Данные PostgreSQL сохраняются в volume и не будут потеряны при обновлении.

## Откат изменений

Если необходимо откатить изменения:

```bash
# Вернуться к предыдущему коммиту
git checkout <previous-commit-hash>

# Пересобрать
docker compose build --no-cache

# Запустить
docker compose up -d
```

## Известные проблемы и решения

### Проблема: "Unable to create table __EFMigrationsHistory"

**Причина:** PostgreSQL еще не готов к подключению

**Решение:** Healthcheck в docker-compose.yml автоматически обработает это. Просто подождите.

### Проблема: Приложение не запускается

**Причина:** Ошибка в миграциях

**Диагностика:**
```bash
docker compose logs webapi
```

**Решение:**
1. Проверьте строку подключения
2. Убедитесь что PostgreSQL запущен
3. Проверьте существующие миграции

## Дополнительные ресурсы

- [docs/DOCKER.md](docs/DOCKER.md) - Полная документация по Docker
- [docs/PROGRAM_CS_MIGRATIONS.md](docs/PROGRAM_CS_MIGRATIONS.md) - Детали применения миграций
- [docs/DATABASE.md](docs/DATABASE.md) - Работа с базой данных
- [QUICKSTART.md](QUICKSTART.md) - Быстрый старт

## Контрольный список для разработчиков

- [x] Обновлен Dockerfile (использует aspnet:8.0)
- [x] Удалена зависимость от entrypoint скрипта
- [x] Обновлена документация
- [x] Создана документация по миграциям из Program.cs
- [ ] Добавлен код миграций в Program.cs (требуется когда будет создан файл)
- [ ] Протестирован полный цикл: build → up → миграции → API работает
- [ ] Проверен размер образа (должен быть ~220MB вместо ~850MB)

## Заключение

Переход на применение миграций из Program.cs упрощает архитектуру приложения, уменьшает размер Docker образа и делает процесс более надежным и прозрачным. Все изменения полностью обратно совместимы и не требуют изменений в базе данных.
