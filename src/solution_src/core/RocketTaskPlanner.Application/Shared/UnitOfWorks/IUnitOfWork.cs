namespace RocketTaskPlanner.Application.Shared.UnitOfWorks;

/// <summary>
/// Контракт для реализации паттерна <inheritdoc cref="IUnitOfWork"/>
/// </summary>
public interface IUnitOfWork : IDisposable
{
    Task BeginTransaction(CancellationToken ct = default);
    Task<Result> SaveChangesAsync(CancellationToken ct = default);
    Task<Result> RollBackTransaction(CancellationToken ct = default);
    Task<Result> CommitTransaction(CancellationToken ct = default);
}
