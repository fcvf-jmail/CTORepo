using Microsoft.OpenApi.Any;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using WebApi.Application.DTOs;
using System;
using System.Collections.Generic;

namespace WebApi.ProjectStartUp.Swagger
{
    /// <summary>
    /// Фильтр для добавления примеров схем в документацию Swagger
    /// </summary>
    public class SchemaExamplesFilter : ISchemaFilter
    {
        private readonly Dictionary<Type, Func<OpenApiObject>> _examples = new()
        {
            [typeof(CreateArticleRequest)] = () => new OpenApiObject
            {
                ["title"] = new OpenApiString("Введение в Clean Architecture"),
                ["content"] = new OpenApiString("Clean Architecture - это архитектурный подход, предложенный Робертом Мартином..."),
                ["tags"] = new OpenApiArray
                {
                    new OpenApiString("Архитектура"),
                    new OpenApiString("Разработка"),
                    new OpenApiString("Паттерны")
                }
            },

            [typeof(UpdateArticleRequest)] = () => new OpenApiObject
            {
                ["title"] = new OpenApiString("Обновленное введение в Clean Architecture"),
                ["content"] = new OpenApiString("Clean Architecture - это архитектурный подход, который помогает разрабатывать масштабируемые приложения..."),
                ["tags"] = new OpenApiArray
                {
                    new OpenApiString("Архитектура"),
                    new OpenApiString("Разработка"),
                    new OpenApiString("Best Practices")
                }
            },

            [typeof(ArticleResponse)] = () => new OpenApiObject
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
            },

            [typeof(SectionResponse)] = () => new OpenApiObject
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
            }
        };

        /// <summary>
        /// Добавляет примеры для схем DTO
        /// </summary>
        public void Apply(OpenApiSchema schema, SchemaFilterContext context)
        {
            if (context == null || schema == null) return;

            if (_examples.TryGetValue(context.Type, out var exampleFactory))
            {
                schema.Example = exampleFactory.Invoke();
            }
        }
    }
}
