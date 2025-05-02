namespace RocketTaskPlanner.Application.UsersContext.Contracts;

public interface IUsersRepository
{
    public IUsersWritableRepository WritableRepository { get; }
    public IUsersReadableRepository ReadableRepository { get; }
}
