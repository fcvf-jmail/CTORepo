# WebApi - ASP.NET Core 8 Web API

## Описание

Проект представляет собой шаблон Web API приложения на базе ASP.NET Core 8 с использованием минимального хостинга (Minimal API).

## Структура решения

Решение организовано по принципам Clean Architecture и разделено на следующие слои:

### Domain (Домен)
`src/WebApi.Domain/`

Содержит основные бизнес-сущности и интерфейсы домена. Этот слой не имеет зависимостей от других проектов и представляет собой ядро приложения.

**Содержит:**
- `Entities/` - базовые сущности домена
- `Interfaces/` - интерфейсы репозиториев и сервисов

### Application (Приложение)
`src/WebApi.Application/`

Содержит бизнес-логику приложения, обработчики команд и запросов,DTO и маппинг.

**Зависимости:** Domain

**Содержит:**
- `Common/` - общие классы и утилиты
- `Interfaces/` - интерфейсы для работы с инфраструктурой

### Infrastructure (Инфраструктура)
`src/WebApi.Infrastructure/`

Содержит реализацию работы с базой данных, внешними сервисами и другой инфраструктуры.

**Зависимости:** Domain, Application

**Содержит:**
- `Data/` - контексты базы данных
- `Repositories/` - реализации репозиториев

### Presentation (Представление)
`src/WebApi.Presentation/`

Web API слой, содержит эндпоинты, контроллеры и настройку приложения.

**Зависимости:** Application, Infrastructure

## Технологии

- .NET 8.0
- ASP.NET Core 8 Minimal API
- Swagger/OpenAPI
- Nullable Reference Types
- Implicit Usings

## Требования

- .NET 8.0 SDK или выше (для локальной разработки)
- Docker и Docker Compose (для запуска в контейнерах)

## Запуск с Docker Compose (рекомендуется)

### Быстрый старт

Запустить приложение вместе с базой данных PostgreSQL одной командой:

```bash
docker compose up --build
```

Эта команда выполнит:
1. Сборку Docker образа приложения
2. Запуск контейнера PostgreSQL
3. Запуск контейнера Web API
4. Автоматическое применение миграций базы данных при старте приложения (из Program.cs)

### Доступ к приложению

После успешного запуска:
- **API:** `http://localhost:5000`
- **Swagger документация:** `http://localhost:5000/swagger`
- **PostgreSQL:** `localhost:5432`
  - База данных: `webapi`
  - Пользователь: `postgres`
  - Пароль: `postgres`

### Переменные окружения

Настройки можно изменить в файле `docker-compose.yml`:

- `POSTGRES_DB` - имя базы данных
- `POSTGRES_USER` - имя пользователя PostgreSQL
- `POSTGRES_PASSWORD` - пароль PostgreSQL
- `ConnectionStrings__DefaultConnection` - строка подключения к БД
- `ASPNETCORE_ENVIRONMENT` - окружение приложения (Production/Development)

## Сборка и запуск (локальная разработка)

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

## Дополнительная документация

- [docs/DOCKER.md](docs/DOCKER.md) - Подробная документация по Docker и Docker Compose
- [docs/DATABASE.md](docs/DATABASE.md) - Документация по работе с базой данных и миграциями
- [docs/ARTICLES_API.md](docs/ARTICLES_API.md) - Документация API для работы со статьями
- [docs/SWAGGER_CONFIGURATION.md](docs/SWAGGER_CONFIGURATION.md) - Конфигурация Swagger/OpenAPI

## SMS Spam Filter (NLP)

В корне репозитория доступен самостоятельный Python-пайплайн `sms_spam_filter.py`, реализующий полный цикл бинарной классификации SMS (spam/ham) на основе TF-IDF и моделей Multinomial Naive Bayes + Logistic Regression.

### Требования

```bash
pip install --break-system-packages pandas numpy seaborn matplotlib scikit-learn nltk python-docx requests
```

### Запуск

```bash
python3 sms_spam_filter.py
```

Скрипт автоматически скачает датасет SMS Spam Collection, выполнит NLP-предобработку (очистка, токенизация, удаление стоп-слов, лемматизация), обучит модели, построит визуализации, выполнит кросс-валидацию и сохранит результаты.

### Артефакты

- `docs/SMS_Spam_Filter_Report.docx` — автоматический отчёт с описанием методологии, метриками, графиками и выводами.
- `ml_outputs/figures/` — PNG-графики (confusion matrix, ROC, распределения и пр.).
- `ml_outputs/artifacts/` — сериализованные модель и TF-IDF в формате pickle.
- `ml_outputs/data/` — кеш исходного датасета (скачивается при первом запуске).

Примеры предсказаний выводятся в консоль, а ключевые события логируются стандартным логгером.
