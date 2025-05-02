using CSharpFunctionalExtensions;
using RocketTaskPlanner.Application.ApplicationTimeContext.Features.SaveTimeZoneDbApiKey;
using RocketTaskPlanner.Application.ApplicationTimeContext.Repository;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Domain.ApplicationTimeContext.Entities.TimeZones;
using RocketTaskPlanner.Infrastructure.TimeZoneDb;
using RocketTaskPlanner.Tests.TestDependencies;

namespace RocketTaskPlanner.Tests.ApplicationTimeContext;

public sealed class ApplicationTimeUseCasesTests : IClassFixture<DefaultTestsFixture>
{
    private readonly DefaultTestsFixture _fixture;

    public ApplicationTimeUseCasesTests(DefaultTestsFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task Add_TimeZoneDbProvider_Record_Success()
    {
        const string token = "N321321DSDSADASH10AEDSADSAHP";
        SaveTimeZoneDbApiKeyUseCase useCase = new(token);

        var useCaseHandler = _fixture.GetService<
            IUseCaseHandler<SaveTimeZoneDbApiKeyUseCase, TimeZoneDbProvider>
        >();

        Result<TimeZoneDbProvider> result = await useCaseHandler.Handle(useCase);
        Assert.True(result.IsSuccess);
    }

    [Fact]
    public async Task Add_TimeZoneDbProviderRecordAndEnsureCreated_Success()
    {
        const string token = "N321321DSDSADASH10AEDSADSAHP";
        SaveTimeZoneDbApiKeyUseCase useCase = new(token);

        var useCaseHandler = _fixture.GetService<
            IUseCaseHandler<SaveTimeZoneDbApiKeyUseCase, TimeZoneDbProvider>
        >();

        Result<TimeZoneDbProvider> result = await useCaseHandler.Handle(useCase);
        Assert.True(result.IsSuccess);

        var repository = _fixture.GetService<IApplicationTimeRepository<TimeZoneDbProvider>>();
        Result<TimeZoneDbProvider> created = await repository.Get();
        Assert.True(created.IsSuccess);

        TimeZoneDbProvider provider = created.Value;
        Assert.Equal(token, provider.Id.Id);
    }

    [Fact]
    public async Task List_Application_Time_Zones_Not_Empty_Success()
    {
        const string token = "N75LSH9ABRHP";
        SaveTimeZoneDbApiKeyUseCase useCase = new(token);

        var useCaseHandler = _fixture.GetService<
            IUseCaseHandler<SaveTimeZoneDbApiKeyUseCase, TimeZoneDbProvider>
        >();

        Result<TimeZoneDbProvider> result = await useCaseHandler.Handle(useCase);
        Assert.True(result.IsSuccess);

        var repository = _fixture.GetService<IApplicationTimeRepository<TimeZoneDbProvider>>();
        Result<TimeZoneDbProvider> created = await repository.Get();
        Assert.True(created.IsSuccess);

        TimeZoneDbProvider provider = created.Value;
        Assert.Equal(token, provider.Id.Id);
        Assert.Empty(provider.TimeZones);

        Result<IReadOnlyList<ApplicationTimeZone>> timesResult = await provider.ProvideTimeZones();
        Assert.True(timesResult.IsSuccess);

        IReadOnlyList<ApplicationTimeZone> times = timesResult.Value;
        Assert.NotNull(times);
        Assert.NotEmpty(times);
    }

    [Fact]
    public async Task List_Application_Time_Zones_And_Save_Success()
    {
        const string token = "N75LSH9ABRHP";
        SaveTimeZoneDbApiKeyUseCase useCase = new(token);

        var useCaseHandler = _fixture.GetService<
            IUseCaseHandler<SaveTimeZoneDbApiKeyUseCase, TimeZoneDbProvider>
        >();

        Result<TimeZoneDbProvider> result = await useCaseHandler.Handle(useCase);
        Assert.True(result.IsSuccess);

        var repository = _fixture.GetService<IApplicationTimeRepository<TimeZoneDbProvider>>();
        Result<TimeZoneDbProvider> created = await repository.Get();
        Assert.True(created.IsSuccess);

        TimeZoneDbProvider provider = created.Value;
        Assert.Equal(token, provider.Id.Id);
        Assert.Empty(provider.TimeZones);

        Result<IReadOnlyList<ApplicationTimeZone>> timesResult = await provider.ProvideTimeZones();
        Assert.True(timesResult.IsSuccess);

        IReadOnlyList<ApplicationTimeZone> times = timesResult.Value;
        Assert.NotNull(times);
        Assert.NotEmpty(times);

        created = await repository.Get();
        Assert.NotEmpty(created.Value.TimeZones);
    }
}
