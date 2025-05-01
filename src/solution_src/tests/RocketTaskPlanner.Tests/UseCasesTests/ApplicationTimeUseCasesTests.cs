using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using RocketTaskPlanner.Application.ApplicationTimeContext.Features.SaveTimeZoneDbApiKey;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Infrastructure.TimeZoneDb;
using RocketTaskPlanner.Presenters.DependencyInjection;

namespace RocketTaskPlanner.Tests.UseCasesTests;

public sealed class ApplicationTimeUseCasesTests
{
    private readonly IServiceScopeFactory _factory;

    public ApplicationTimeUseCasesTests()
    {
        IServiceCollection services = new ServiceCollection();
        services.Inject();
        IServiceProvider provider = services.BuildServiceProvider();
        _factory = provider.GetRequiredService<IServiceScopeFactory>();
    }

    [Fact]
    public async Task Add_TimeZone_Db_Token()
    {
        IServiceScope scope = _factory.CreateScope();
        IServiceProvider provider = scope.ServiceProvider;
        IUseCaseHandler<SaveTimeZoneDbApiKeyUseCase, TimeZoneDbProvider> handler =
            provider.GetRequiredService<
                IUseCaseHandler<SaveTimeZoneDbApiKeyUseCase, TimeZoneDbProvider>
            >();
        SaveTimeZoneDbApiKeyUseCase useCase = new SaveTimeZoneDbApiKeyUseCase(
            "DSADSAASD321321321ASFSA"
        );
        Result<TimeZoneDbProvider> result = await handler.Handle(useCase);
    }
}
