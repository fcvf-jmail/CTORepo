using Microsoft.EntityFrameworkCore;

namespace WebApi.Infrastructure.Data;

/// <summary>
/// Контекст базы данных приложения
/// </summary>
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Здесь будут конфигурации сущностей
    }
}
