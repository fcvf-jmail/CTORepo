using Microsoft.EntityFrameworkCore;
using WebApi.Domain.Entities;
using WebApi.Domain.Interfaces;
using WebApi.Infrastructure.Data;

namespace WebApi.Infrastructure.Repositories;

/// <summary>
/// Реализация репозитория для работы с тегами
/// </summary>
public class TagRepository : ITagRepository
{
    private readonly ApplicationDbContext _context;

    /// <summary>
    /// Конструктор репозитория тегов
    /// </summary>
    /// <param name="context">Контекст базы данных</param>
    public TagRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Получить тег по идентификатору
    /// </summary>
    public async Task<Tag?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Tags.FindAsync(new object[] { id }, cancellationToken);
    }

    /// <summary>
    /// Получить тег по нормализованному имени (без учета регистра)
    /// </summary>
    public async Task<Tag?> GetByNormalizedNameAsync(string normalizedName, CancellationToken cancellationToken = default)
    {
        return await _context.Tags
            .FirstOrDefaultAsync(t => t.NormalizedName == normalizedName, cancellationToken);
    }

    /// <summary>
    /// Получить теги по списку нормализованных имен
    /// </summary>
    public async Task<List<Tag>> GetByNormalizedNamesAsync(List<string> normalizedNames, CancellationToken cancellationToken = default)
    {
        return await _context.Tags
            .Where(t => normalizedNames.Contains(t.NormalizedName))
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Создать новый тег
    /// </summary>
    public async Task CreateAsync(Tag tag, CancellationToken cancellationToken = default)
    {
        await _context.Tags.AddAsync(tag, cancellationToken);
    }

    /// <summary>
    /// Обновить существующий тег
    /// </summary>
    public Task UpdateAsync(Tag tag, CancellationToken cancellationToken = default)
    {
        _context.Tags.Update(tag);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Удалить тег по идентификатору
    /// </summary>
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var tag = await _context.Tags.FindAsync(new object[] { id }, cancellationToken);
        if (tag != null)
        {
            _context.Tags.Remove(tag);
        }
    }

    /// <summary>
    /// Проверить существование тега по имени (без учета регистра)
    /// </summary>
    public async Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        var normalizedName = name.ToLowerInvariant();
        return await _context.Tags
            .AnyAsync(t => t.NormalizedName == normalizedName, cancellationToken);
    }

    /// <summary>
    /// Сохранить изменения в базе данных
    /// </summary>
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
