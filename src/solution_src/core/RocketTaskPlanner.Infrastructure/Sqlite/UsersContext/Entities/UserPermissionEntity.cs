namespace RocketTaskPlanner.Infrastructure.Sqlite.UsersContext.Entities;

/// <summary>
/// Dao модель права пользователя
/// </summary>
public sealed class UserPermissionEntity
{
    public string Id { get; set; } = string.Empty;
    public long UserId { get; set; }
    public string Name { get; set; } = string.Empty;
}
