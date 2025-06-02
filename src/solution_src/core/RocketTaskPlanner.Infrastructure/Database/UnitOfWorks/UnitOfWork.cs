using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore.Storage;
using RocketTaskPlanner.Application.Shared.UnitOfWorks;

namespace RocketTaskPlanner.Infrastructure.Database.UnitOfWorks;

/// <summary>
/// Реализация паттерна Unit Of Work для соблюдения транзакции между разными БД Sqlite
/// </summary>
public sealed class UnitOfWork : IUnitOfWork
{
    private const string Context = nameof(IUnitOfWork);
    private readonly RocketTaskPlannerDbContext _context;
    private readonly Serilog.ILogger _logger;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(RocketTaskPlannerDbContext context, Serilog.ILogger logger)
    {
        _context = context;
        _logger = logger;
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }

    public async Task BeginTransaction(CancellationToken ct = default) =>
        _transaction = await _context.Database.BeginTransactionAsync(ct);

    public async Task<Result> SaveChangesAsync(CancellationToken ct = default)
    {
        if (_transaction == null)
        {
            _logger.Error("{Context}. Transaction was not started.", Context);
            return Result.Failure("Transaction was not started.");
        }

        try
        {
            await _context.SaveChangesAsync(ct);
            return Result.Success();
        }
        catch
        {
            return Result.Failure("Ошибка при операции транзакции");
        }
    }

    public async Task<Result> RollBackTransaction(CancellationToken ct = default)
    {
        if (_transaction == null)
        {
            _logger.Error("{Context}. Transaction was not started.", Context);
            return Result.Failure("Transaction was not started.");
        }
        
        await _transaction.RollbackAsync(ct);
        return Result.Success();
    }

    public async Task<Result> CommitTransaction(CancellationToken ct = default)
    {
        if (_transaction == null)
        {
            _logger.Error("{Context}. Transaction was not started.", Context);
            return Result.Failure("Transaction was not started.");
        }
        
        try
        {
            _logger.Information("{Context}. Committing transaction", Context);
            await _transaction.CommitAsync(ct);
            _logger.Information("{Context}. Transaction successfully commited", Context);
            return Result.Success();
        }
        catch(Exception ex)
        {
            await _transaction.RollbackAsync(ct);
            _logger.Fatal("Transaction error: {Exception}", ex.Message);
            _logger.Error("{Context}. Transaction was not commited.", Context);
            return Result.Failure("Ошибка при операции транзакции");
        }
    }
}
