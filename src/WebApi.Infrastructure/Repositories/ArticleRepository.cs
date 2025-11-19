using Microsoft.EntityFrameworkCore;
using WebApi.Domain.Entities;
using WebApi.Domain.Interfaces;
using WebApi.Infrastructure.Data;

namespace WebApi.Infrastructure.Repositories;

/// <summary>
/// Реализация репозитория для работы со статьями
/// </summary>
/// <remarks>
public class ArticleRepository(ApplicationDbContext context) : IArticleRepository
{
    private readonly ApplicationDbContext _context = context;

    /// <summary>
    /// Получить статью по идентификатору
    /// </summary>
    public async Task<Article?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Articles
            .Include(a => a.Tags)
            .FirstOrDefaultAsync(a => a.Id == id, cancellationToken);
    }

    /// <summary>
    /// Получить все статьи
    /// </summary>
    public async Task<IEnumerable<Article>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _context.Articles
            .Include(a => a.Tags)
            .ToListAsync(cancellationToken);
    }

    /// <summary>
    /// Создать новую статью
    /// </summary>
    public async Task CreateAsync(Article article, CancellationToken cancellationToken = default)
    {
        await _context.Articles.AddAsync(article, cancellationToken);
    }

    /// <summary>
    /// Обновить существующую статью
    /// </summary>
    public Task UpdateAsync(Article article, CancellationToken cancellationToken = default)
    {
        _context.Articles.Update(article);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Удалить статью по идентификатору
    /// </summary>
    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var article = await _context.Articles.FindAsync(new object[] { id }, cancellationToken);
        if (article != null)
        {
            _context.Articles.Remove(article);
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
