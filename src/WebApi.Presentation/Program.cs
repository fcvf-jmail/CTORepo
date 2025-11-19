using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using WebApi.Application.Interfaces;
using WebApi.Infrastructure.Data;
using WebApi.Infrastructure.Services;
using WebApi.Presentation.Swagger;

// Точка входа в приложение Web API
var builder = WebApplication.CreateBuilder(args);

// Добавление сервисов в контейнер DI
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<ISectionService, SectionService>();
builder.Services.AddScoped<IArticleService, ArticleService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "Web API для управления статьями",
        Description = @"REST API для управления статьями и разделами.

**Основные возможности:**
- Создание, чтение, обновление и удаление статей
- Автоматическое управление разделами на основе тегов
- Автоматическая сортировка и дедупликация тегов
- Получение списка разделов и статей внутри них

**Особенности работы с тегами:**
- Теги дедуплицируются без учета регистра
- В ответах теги всегда отсортированы по алфавиту
- Раздел определяется уникальным набором тегов

**Особенности сортировки:**
- Разделы сортируются по количеству статей (убывание)
- Статьи в разделе сортируются по дате обновления/создания (убывание)
- Все даты представлены в формате UTC"
    });

    // Подключение XML-комментариев из проекта Presentation
    var presentationXmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var presentationXmlPath = Path.Combine(AppContext.BaseDirectory, presentationXmlFile);
    if (File.Exists(presentationXmlPath))
    {
        options.IncludeXmlComments(presentationXmlPath, includeControllerXmlComments: true);
    }

    // Подключение XML-комментариев из проекта Application
    var applicationXmlFile = "WebApi.Application.xml";
    var applicationXmlPath = Path.Combine(AppContext.BaseDirectory, applicationXmlFile);
    if (File.Exists(applicationXmlPath))
    {
        options.IncludeXmlComments(applicationXmlPath, includeControllerXmlComments: true);
    }

    // Добавление фильтров
    options.OperationFilter<ResponseExamplesFilter>();
    options.SchemaFilter<SchemaExamplesFilter>();
    options.DocumentFilter<TagSortingFilter>();

    // Настройка отображения enum как строк
    options.UseInlineDefinitionsForEnums();
});

var app = builder.Build();

using var scope = app.Services.CreateScope();
var services = scope.ServiceProvider;
try
{
    var context = services.GetRequiredService<ApplicationDbContext>();
    await context.Database.MigrateAsync();
}
catch (Exception ex)
{
    var logger = services.GetRequiredService<ILogger<Program>>();
    logger.LogError(ex, "Ошибка при применении миграций");
    throw; // Опционально: прервать запуск при ошибке миграции
}

// Настройка конвейера обработки HTTP-запросов
app.UseSwagger();

app.UseSwaggerUI(options =>
{
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "Web API для управления статьями v1");
    options.DocumentTitle = "Web API - Документация";
    options.RoutePrefix = "swagger";
});

// Регистрация контроллеров
app.MapControllers();

app.Run();
