using RocketTaskPlanner.Application.PermissionsContext.Features.AddPermission;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Domain.PermissionsContext;

namespace RocketTaskPlanner.Telegram.PermissionsSetup;

public static class PermissionsSetupExtensions
{
    public static async Task RegisterBasicPermissions(this IHost host)
    {
        IServiceScopeFactory factory = host.Services.GetRequiredService<IServiceScopeFactory>();
        await using AsyncServiceScope scope = factory.CreateAsyncScope();
        await AddConfigEditorPermission(scope);
        await AddCreateTaskPermission(scope);
    }

    private static async Task AddConfigEditorPermission(this AsyncServiceScope scope)
    {
        AddPermissionUseCase useCase = new(PermissionNames.EditConfiguration);
        var handler = scope.ServiceProvider.GetRequiredService<
            IUseCaseHandler<AddPermissionUseCase, Permission>
        >();
        await handler.Handle(useCase);
    }

    private static async Task AddCreateTaskPermission(this AsyncServiceScope scope)
    {
        AddPermissionUseCase useCase = new(PermissionNames.CreateTasks);
        var handler = scope.ServiceProvider.GetRequiredService<
            IUseCaseHandler<AddPermissionUseCase, Permission>
        >();
        await handler.Handle(useCase);
    }
}
