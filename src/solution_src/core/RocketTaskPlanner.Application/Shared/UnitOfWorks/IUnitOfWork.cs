namespace RocketTaskPlanner.Application.Shared.UnitOfWorks;

/// <summary>
/// Контракт для реализации паттерна <inheritdoc cref="IUnitOfWork"/>
/// </summary>
public interface IUnitOfWork : IDisposable
{
    /// <summary>
    /// Добавить команду из репозитория (SQL запрос)
    /// <param name="repository">
    ///     <inheritdoc cref="IRepository"/>
    /// </param>
    /// <param name="command">
    ///     <inheritdoc cref="UnitOfWorkCommand"/>
    /// </param>
    /// </summary>
    void AddCommand(IRepository repository, UnitOfWorkCommand command);

    /// <summary>
    /// Выполнить все команды в IUnitOfWork
    /// </summary>
    /// <returns></returns>
    Task Process();

    /// <summary>
    /// Завершить транзакцию
    /// </summary>
    /// <returns>Success если транзакция завершилась успешно или Failure</returns>
    Result TryCommit();
}
