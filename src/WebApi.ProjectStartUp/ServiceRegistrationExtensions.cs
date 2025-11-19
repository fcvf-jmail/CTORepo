namespace WebApi.ProjectStartUp;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using WebApi.Infrastructure.Data;
using WebApi.Domain.Interfaces;
using WebApi.Infrastructure.Repositories;
using WebApi.Application.Interfaces;
using WebApi.Infrastructure.Services;
using Microsoft.Extensions.Configuration;

public static class ServiceRegistrationExtensions
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
}
