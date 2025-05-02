namespace RocketTaskPlanner.Infrastructure.Sqlite.UsersContext.Entities;

public sealed class UserPermissionEntity
{
    public string Id { get; set; }
    public long UserId { get; set; }
    public string Name { get; set; } = string.Empty;
}
