using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApi.Presentation.Swagger;

/// <summary>
/// Фильтр для добавления примеров ответов и описаний кодов состояния
/// </summary>
public class ResponseExamplesFilter : IOperationFilter
{
    /// <summary>
    /// Добавляет описания кодов ответов для всех операций
    /// </summary>
    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        if (!operation.Responses.ContainsKey("200"))
        {
            operation.Responses.TryAdd("200", new OpenApiResponse
            {
                Description = "Успешное выполнение запроса"
            });
        }
        else
        {
            operation.Responses["200"].Description = "Успешное выполнение запроса";
        }

        if (operation.Responses.ContainsKey("201"))
        {
            operation.Responses["201"].Description = "Ресурс успешно создан";
        }

        if (operation.Responses.ContainsKey("204"))
        {
            operation.Responses["204"].Description = "Успешное выполнение, без тела ответа";
        }

        if (operation.Responses.ContainsKey("400"))
        {
            operation.Responses["400"].Description = "Неверный формат запроса или нарушение правил валидации";
        }

        if (operation.Responses.ContainsKey("404"))
        {
            operation.Responses["404"].Description = "Запрашиваемый ресурс не найден";
        }

        if (!operation.Responses.ContainsKey("500"))
        {
            operation.Responses.TryAdd("500", new OpenApiResponse
            {
                Description = "Внутренняя ошибка сервера"
            });
        }
        else
        {
            operation.Responses["500"].Description = "Внутренняя ошибка сервера";
        }
    }
}
