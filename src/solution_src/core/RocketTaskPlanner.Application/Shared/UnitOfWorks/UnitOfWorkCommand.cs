using System.Data;

namespace RocketTaskPlanner.Application.Shared.UnitOfWorks;

/// <summary>
/// SQL команда для UnitOfWork
/// </summary>
/// <param name="Command">SQL команда</param>
public sealed record UnitOfWorkCommand(Func<IDbConnection, Task> Command);
