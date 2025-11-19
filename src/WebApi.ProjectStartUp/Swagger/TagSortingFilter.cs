using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApi.Presentation.Swagger;

/// <summary>
/// Фильтр для сортировки тегов в документации Swagger
/// </summary>
public class TagSortingFilter : IDocumentFilter
{
    /// <summary>
    /// Применяет сортировку тегов по алфавиту
    /// </summary>
    public void Apply(OpenApiDocument swaggerDoc, DocumentFilterContext context)
    {
        swaggerDoc.Tags = swaggerDoc.Tags
            .OrderBy(tag => tag.Name)
            .ToList();
    }
}
