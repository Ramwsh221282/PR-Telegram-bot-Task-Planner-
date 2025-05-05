using System.Data;

namespace RocketTaskPlanner.Application.Shared.UnitOfWorks;

public sealed record UnitOfWorkCommand(Func<IDbConnection, Task> Command);
