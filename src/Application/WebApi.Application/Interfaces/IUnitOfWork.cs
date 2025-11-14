namespace WebApi.Application.Interfaces;

/// <summary>
/// Интерфейс паттерна Unit of Work для управления транзакциями
/// </summary>
public interface IUnitOfWork
{
    /// <summary>
    /// Сохранить все изменения в рамках текущей транзакции
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    /// <returns>Количество затронутых записей</returns>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Начать новую транзакцию
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Зафиксировать текущую транзакцию
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Откатить текущую транзакцию
    /// </summary>
    /// <param name="cancellationToken">Токен отмены операции</param>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
