namespace RocketTaskPlanner.Application.Shared.UnitOfWorks;

public interface IUnitOfWork : IDisposable
{
    void AddCommand(IRepository repository, UnitOfWorkCommand command);

    Task Process();

    Result TryCommit();
}
