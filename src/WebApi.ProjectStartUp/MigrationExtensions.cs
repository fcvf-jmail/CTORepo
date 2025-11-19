using Microsoft.EntityFrameworkCore;
using WebApi.Infrastructure.Data;

namespace WebApi.ProjectStartUp;

public static class MigrationExtensions
{
    public static async Task MigrateDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var services = scope.ServiceProvider;
        try
        {
            var context = services.GetRequiredService<ApplicationDbContext>();
            await context.Database.MigrateAsync();
        }
        catch (Exception ex)
        {
            var logger = services.GetRequiredService<ILogger>();
            logger.LogError(ex, "Ошибка при применении миграций");
            throw;
        }
    }
}
