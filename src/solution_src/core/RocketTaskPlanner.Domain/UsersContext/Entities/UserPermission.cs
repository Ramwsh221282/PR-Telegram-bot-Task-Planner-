namespace RocketTaskPlanner.Domain.UsersContext.Entities;

public sealed class UserPermission
{
    public User User { get; } = null!;
    public string Name { get; } = null!;
    public Guid Id { get; }

    private UserPermission() { } // ef core

    public UserPermission(User user, string name, Guid id)
    {
        User = user;
        Name = name;
        Id = id;
    }
}
