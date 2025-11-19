using System.Reflection;
using Microsoft.EntityFrameworkCore;
using WebApi.Application.Interfaces;
using WebApi.Domain.Interfaces;
using WebApi.Infrastructure.Data;
using WebApi.Infrastructure.Repositories;
using WebApi.Infrastructure.Services;
using WebApi.Presentation.Swagger;
using WebApi.ProjectStartUp.Swagger;
using Microsoft.OpenApi.Models;

namespace WebApi.ProjectStartUp
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddCustomServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(configuration.GetConnectionString("DefaultConnection")));

            services.AddScoped<IArticleRepository, ArticleRepository>();
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<ISectionRepository, SectionRepository>();

            services.AddScoped<ISectionService, SectionService>();
            services.AddScoped<IArticleService, ArticleService>();

            return services;
        }

        public static IServiceCollection AddCustomSwagger(this IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
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

                var presentationXmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var presentationXmlPath = Path.Combine(AppContext.BaseDirectory, presentationXmlFile);
                if (File.Exists(presentationXmlPath))
                {
                    options.IncludeXmlComments(presentationXmlPath, includeControllerXmlComments: true);
                }

                var applicationXmlFile = "WebApi.Application.xml";
                var applicationXmlPath = Path.Combine(AppContext.BaseDirectory, applicationXmlFile);
                if (File.Exists(applicationXmlPath))
                {
                    options.IncludeXmlComments(applicationXmlPath, includeControllerXmlComments: true);
                }

                options.OperationFilter<ResponseExamplesFilter>();
                options.SchemaFilter<SchemaExamplesFilter>();
                options.DocumentFilter<TagSortingFilter>();
                options.UseInlineDefinitionsForEnums();
            });

            return services;
        }

        public static IApplicationBuilder UseCustomSwagger(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("/swagger/v1/swagger.json", "Web API для управления статьями v1");
                options.DocumentTitle = "Web API - Документация";
                options.RoutePrefix = "swagger";
            });

            return app;
        }
    }
}
