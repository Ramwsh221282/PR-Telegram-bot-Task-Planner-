using System.Data;
using CSharpFunctionalExtensions;
using RocketTaskPlanner.Application.Shared.UnitOfWorks;

namespace RocketTaskPlanner.Infrastructure.Sqlite.UnitOfWorks;

public sealed class UnitOfWork : IUnitOfWork
{
    private readonly Dictionary<UnitOfWorkContext, List<UnitOfWorkCommand>> _commands = [];

    private bool _hasErrors;

    public void AddCommand(IRepository repository, UnitOfWorkCommand command)
    {
        _commands[ExistingOrNewContext(repository)].Add(command);
    }

    public async Task Process()
    {
        foreach (KeyValuePair<UnitOfWorkContext, List<UnitOfWorkCommand>> context in _commands)
        {
            UnitOfWorkContext transactionContext = context.Key;
            List<UnitOfWorkCommand> transactionCommands = context.Value;
            if (await ExecuteCommandsInSafe(transactionContext, transactionCommands))
                continue;

            break;
        }
    }

    public Result TryCommit()
    {
        if (_hasErrors)
        {
            RollBackTransaction();
            DisposeTransactions();
            return Result.Failure("Ошибка при транзакции");
        }

        foreach (var context in _commands)
        {
            try
            {
                context.Key.Transaction.Commit();
            }
            catch
            {
                RollBackTransaction();
                DisposeTransactions();
                return Result.Failure("Ошибка при транзакции");
            }
        }

        DisposeTransactions();
        return Result.Success();
    }

    private UnitOfWorkContext ExistingOrNewContext(IRepository repository)
    {
        IDbConnection connection = repository.CreateConnection();
        var existing = _commands.Keys.FirstOrDefault(c =>
            c.Connection.ConnectionString == connection.ConnectionString
        );

        if (existing != null)
        {
            connection.Dispose();
            return existing;
        }

        IDbTransaction transaction = connection.BeginTransaction();
        UnitOfWorkContext context = new(connection, transaction);
        _commands.Add(context, []);
        return context;
    }

    private async Task<bool> ExecuteCommandsInSafe(
        UnitOfWorkContext transactionContext,
        List<UnitOfWorkCommand> commands
    )
    {
        try
        {
            foreach (UnitOfWorkCommand command in commands)
                await command.Command(transactionContext.Connection);

            _hasErrors = false;
            return true;
        }
        catch
        {
            _hasErrors = true;
            return false;
        }
    }

    private void RollBackTransaction()
    {
        foreach (UnitOfWorkContext key in _commands.Keys)
            key.Transaction.Rollback();
    }

    private void DisposeTransactions()
    {
        foreach (UnitOfWorkContext key in _commands.Keys)
        {
            key.Connection.Dispose();
            key.Transaction.Dispose();
        }

        _commands.Clear();
    }

    public void Dispose()
    {
        if (_commands.Count > 0)
            DisposeTransactions();
    }
}
