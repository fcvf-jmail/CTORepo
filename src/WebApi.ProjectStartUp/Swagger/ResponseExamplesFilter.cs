using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace WebApi.Presentation.Swagger
{
    /// <summary>
    /// Фильтр для добавления примеров ответов и описаний кодов состояния
    /// </summary>
    public class ResponseExamplesFilter : IOperationFilter
    {
        private static readonly Dictionary<string, string> ResponseDescriptions = new()
        {
            { "200", "Успешное выполнение запроса" },
            { "201", "Ресурс успешно создан" },
            { "204", "Успешное выполнение, без тела ответа" },
            { "400", "Неверный формат запроса или нарушение правил валидации" },
            { "404", "Запрашиваемый ресурс не найден" },
            { "500", "Внутренняя ошибка сервера" }
        };

        /// <summary>
        /// Добавляет описания кодов ответов для всех операций
        /// </summary>
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            foreach (var (statusCode, description) in ResponseDescriptions)
            {
                if (operation.Responses.TryGetValue(statusCode, out OpenApiResponse? value))
                {
                    value.Description = description;
                }
                else
                {
                    operation.Responses[statusCode] = new OpenApiResponse { Description = description };
                }
            }
        }
    }
}
