using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WebApi.Application.DTOs;

namespace WebApi.Presentation.Swagger;

/// <summary>
/// Фильтр для добавления примеров схем в документацию Swagger
/// </summary>
public class SchemaExamplesFilter : ISchemaFilter
{
    /// <summary>
    /// Добавляет примеры для схем DTO
    /// </summary>
    public void Apply(OpenApiSchema schema, SchemaFilterContext context)
    {
        if (context.Type == typeof(CreateArticleRequest))
        {
            schema.Example = new OpenApiObject
            {
                ["title"] = new OpenApiString("Введение в Clean Architecture"),
                ["content"] = new OpenApiString("Clean Architecture - это архитектурный подход, предложенный Робертом Мартином..."),
                ["tags"] = new OpenApiArray
                {
                    new OpenApiString("Архитектура"),
                    new OpenApiString("Разработка"),
                    new OpenApiString("Паттерны")
                }
            };
        }
        else if (context.Type == typeof(UpdateArticleRequest))
        {
            schema.Example = new OpenApiObject
            {
                ["title"] = new OpenApiString("Обновленное введение в Clean Architecture"),
                ["content"] = new OpenApiString("Clean Architecture - это архитектурный подход, который помогает разрабатывать масштабируемые приложения..."),
                ["tags"] = new OpenApiArray
                {
                    new OpenApiString("Архитектура"),
                    new OpenApiString("Разработка"),
                    new OpenApiString("Best Practices")
                }
            };
        }
        else if (context.Type == typeof(ArticleResponse))
        {
            schema.Example = new OpenApiObject
            {
                ["id"] = new OpenApiString("3fa85f64-5717-4562-b3fc-2c963f66afa6"),
                ["title"] = new OpenApiString("Введение в Clean Architecture"),
                ["content"] = new OpenApiString("Clean Architecture - это архитектурный подход, предложенный Робертом Мартином..."),
                ["sectionId"] = new OpenApiString("7fa85f64-5717-4562-b3fc-2c963f66afa9"),
                ["tags"] = new OpenApiArray
                {
                    new OpenApiString("Архитектура"),
                    new OpenApiString("Паттерны"),
                    new OpenApiString("Разработка")
                },
                ["createdAt"] = new OpenApiString("2024-01-15T10:30:00Z"),
                ["updatedAt"] = new OpenApiString("2024-01-16T14:45:00Z")
            };
        }
        else if (context.Type == typeof(SectionResponse))
        {
            schema.Example = new OpenApiObject
            {
                ["id"] = new OpenApiString("7fa85f64-5717-4562-b3fc-2c963f66afa9"),
                ["name"] = new OpenApiString("Архитектура, Паттерны, Разработка"),
                ["tags"] = new OpenApiArray
                {
                    new OpenApiString("Архитектура"),
                    new OpenApiString("Паттерны"),
                    new OpenApiString("Разработка")
                },
                ["articleCount"] = new OpenApiInteger(5),
                ["createdAt"] = new OpenApiString("2024-01-15T10:30:00Z"),
                ["updatedAt"] = new OpenApiString("2024-01-20T09:15:00Z")
            };
        }
    }
}
