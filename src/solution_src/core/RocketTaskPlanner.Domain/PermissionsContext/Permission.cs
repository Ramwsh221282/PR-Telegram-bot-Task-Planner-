namespace RocketTaskPlanner.Domain.PermissionsContext;

public sealed class Permission
{
    public required Guid Id { get; set; }
    public required string Name { get; set; }
}

public static class PermissionNames
{
    public const string EditConfiguration = "EditConfiguration";
    public const string CreateTasks = "CreateTasks";
}
