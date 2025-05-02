using RocketTaskPlanner.Domain.UsersContext.ValueObjects;

namespace RocketTaskPlanner.Domain.UsersContext.Entities;

public sealed class UserPermission
{
    public User User { get; } = null!;
    public UserId UserId { get; }
    public string Name { get; } = null!;
    public Guid Id { get; }

    private UserPermission() { } // ef core

    public UserPermission(User user, string name, Guid id)
    {
        User = user;
        UserId = user.Id;
        Name = name;
        Id = id;
    }
}
