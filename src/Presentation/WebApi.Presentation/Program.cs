/// <summary>
/// Точка входа в приложение Web API
/// </summary>
var builder = WebApplication.CreateBuilder(args);

// Добавление сервисов в контейнер DI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Настройка конвейера обработки HTTP-запросов
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var summaries = new[]
{
    "Морозно", "Холодно", "Прохладно", "Умеренно", "Тепло", "Жарко", "Знойно", "Очень жарко"
};

/// <summary>
/// Эндпоинт для получения прогноза погоды
/// </summary>
app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

/// <summary>
/// Модель прогноза погоды
/// </summary>
/// <param name="Date">Дата прогноза</param>
/// <param name="TemperatureC">Температура в градусах Цельсия</param>
/// <param name="Summary">Краткое описание погоды</param>
record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    /// <summary>
    /// Температура в градусах Фаренгейта
    /// </summary>
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}
