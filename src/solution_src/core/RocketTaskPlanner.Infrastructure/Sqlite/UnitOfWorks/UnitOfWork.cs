using System.Data;
using CSharpFunctionalExtensions;
using RocketTaskPlanner.Application.Shared.UnitOfWorks;

namespace RocketTaskPlanner.Infrastructure.Sqlite.UnitOfWorks;

/// <summary>
/// Реализация паттерна Unit Of Work для соблюдения транзакции между разными БД Sqlite
/// </summary>
public sealed class UnitOfWork : IUnitOfWork
{
    /// <summary>
    /// Команды - запросы SQL. Добавляются сюда из репозиториев <inheritdoc cref="IRepository"/>
    /// </summary>
    private readonly Dictionary<UnitOfWorkContext, List<UnitOfWorkCommand>> _commands = [];

    /// <summary>
    /// Флаг для проверки наличия ошибок в операциях
    /// </summary>
    private bool _hasErrors;

    /// <summary>
    /// Создания контекста SQL команд при транзакции из одной базы данных.
    /// </summary>
    /// <param name="repository">
    ///     <inheritdoc cref="IRepository"/>
    /// </param>
    /// <param name="command">
    /// Команда SQL запроса
    /// </param>
    public void AddCommand(IRepository repository, UnitOfWorkCommand command)
    {
        _commands[ExistingOrNewContext(repository)].Add(command);
    }

    /// <summary>
    /// Выполнение всех команд в транзакции
    /// </summary>
    public async Task Process()
    {
        foreach (KeyValuePair<UnitOfWorkContext, List<UnitOfWorkCommand>> context in _commands)
        {
            // контекст транзакции.
            UnitOfWorkContext transactionContext = context.Key;

            // список команд транзакции
            List<UnitOfWorkCommand> transactionCommands = context.Value;

            // если команда выполнилась без ошибок - продолжение выполнение команд
            if (await ExecuteCommandsInSafe(transactionContext, transactionCommands))
                continue;

            // если команда с ошибкой, остановка выполнения дальнейших команд.
            break;
        }
    }

    /// <summary>
    /// Попытка завершить транзакцию
    /// </summary>
    /// <returns>Success если ошибок нет. Failure если ошибки в транзакции</returns>
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
                RollBackTransaction(); // откат изменений
                DisposeTransactions(); // освобождение соединения и транзакции

                return Result.Failure("Ошибка при транзакции");
            }
        }

        DisposeTransactions(); // освобождение соединения и транзакции
        return Result.Success();
    }

    /// <summary>
    /// Создание контекста транзакции, либо возврат текущего контекста из словаря.
    /// Текущий контекст выбирается на основе строки соединения с БД.
    /// Если транзакция с существующим соединением есть - значит она есть.
    /// Если нет - создание нового контекста транзакции
    /// <param name="repository"><inheritdoc cref="IRepository"/></param>
    /// <returns></returns>
    /// </summary>
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

    /// <summary>
    /// Выполнение команд SQL запросов.
    /// Если хотя бы одна команда выполняется с ошибкой, то прекращение выполнения следующих команд.
    /// </summary>
    /// <param name="transactionContext">
    ///     <inheritdoc cref="UnitOfWorkContext"/>
    /// </param>
    /// <param name="commands">
    ///     <inheritdoc cref="UnitOfWorkCommand"/>
    /// </param>
    /// <returns>True если без ошибки, False если с ошибкой.</returns>
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

    /// <summary>
    /// Откат изменений
    /// </summary>
    private void RollBackTransaction()
    {
        foreach (UnitOfWorkContext key in _commands.Keys)
            key.Transaction.Rollback();
    }

    /// <summary>
    /// Высвобождение ресурсов (высвобождение соединения и транзакций)
    /// Очистка словаря с транзакциями
    /// </summary>
    private void DisposeTransactions()
    {
        foreach (UnitOfWorkContext key in _commands.Keys)
        {
            key.Connection.Dispose();
            key.Transaction.Dispose();
        }

        _commands.Clear();
    }

    /// <summary>
    /// Высвобождение ресурсов <inheritdoc cref="DisposeTransactions"/>
    /// </summary>
    public void Dispose()
    {
        if (_commands.Count > 0)
            DisposeTransactions();
    }
}
