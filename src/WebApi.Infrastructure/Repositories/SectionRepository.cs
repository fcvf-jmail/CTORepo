using Microsoft.EntityFrameworkCore;
using WebApi.Domain.Entities;
using WebApi.Domain.Interfaces;
using WebApi.Infrastructure.Data;

namespace WebApi.Infrastructure.Repositories;

/// <summary>
/// Реализация репозитория для работы с разделами
/// </summary>
public class SectionRepository(ApplicationDbContext context) : ISectionRepository
{
    private readonly ApplicationDbContext _context = context;

    /// <summary>
    /// Получить раздел по идентификатору
    /// </summary>
    public async Task<Section?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sections
            .Include(s => s.Tags)
            .Include(s => s.Articles)
                .ThenInclude(a => a.Tags)
            .FirstOrDefaultAsync(s => s.Id == id, cancellationToken);
    }

    /// <summary>
    /// Получить все разделы
    /// </summary>
    public async Task<IEnumerable<Section>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Sections
            .Include(s => s.Tags)
            .Include(s => s.Articles)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Найти раздел по набору нормализованных имен тегов
    /// </summary>
    public async Task<Section?> GetByTagsAsync(List<string> normalizedTagNames, CancellationToken cancellationToken = default)
    {
        if (normalizedTagNames.Count == 0)
        {
            return await _context.Sections
                .Include(s => s.Tags)
                .FirstOrDefaultAsync(s => s.Tags.Count == 0, cancellationToken);
        }

        var sections = await _context.Sections
            .Include(s => s.Tags)
            .Where(s => s.Tags.Count == normalizedTagNames.Count)
            .ToListAsync(cancellationToken);

        foreach (var section in sections)
        {
            var sectionTagNames = section.Tags
                .Select(t => t.NormalizedName)
                .OrderBy(n => n)
                .ToList();

            if (normalizedTagNames.SequenceEqual(sectionTagNames))
            {
                return section;
            }
        }

        return null;
    }

    /// <summary>
    /// Создать новый раздел
    /// </summary>
    public async Task CreateAsync(Section section, CancellationToken cancellationToken = default)
    {
        await _context.Sections.AddAsync(section, cancellationToken);
    }

    /// <summary>
    /// Обновить существующий раздел
    /// </summary>
    public Task UpdateAsync(Section section, CancellationToken cancellationToken = default)
    {
        _context.Sections.Update(section);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Удалить раздел по идентификатору
    /// </summary>
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var section = await _context.Sections.FindAsync(new object[] { id }, cancellationToken);
        if (section != null)
        {
            _context.Sections.Remove(section);
        }
    }

    /// <summary>
    /// Сохранить изменения в базе данных
    /// </summary>
    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}
