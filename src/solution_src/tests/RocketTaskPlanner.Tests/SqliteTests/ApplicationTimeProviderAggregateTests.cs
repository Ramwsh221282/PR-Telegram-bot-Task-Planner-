using CSharpFunctionalExtensions;
using Microsoft.Extensions.DependencyInjection;
using RocketTaskPlanner.Application.ApplicationTimeContext.Repository;
using RocketTaskPlanner.Domain.ApplicationTimeContext.Entities.TimeZones;
using RocketTaskPlanner.Infrastructure.TimeZoneDb;
using RocketTaskPlanner.Presenters.DependencyInjection;

namespace RocketTaskPlanner.Tests.SqliteTests;

public sealed class ApplicationTimeProviderAggregateTests
{
    private readonly IServiceScopeFactory _factory;

    public ApplicationTimeProviderAggregateTests()
    {
        IServiceCollection services = new ServiceCollection();
        services.Inject();
        IServiceProvider provider = services.BuildServiceProvider();
        _factory = provider.GetRequiredService<IServiceScopeFactory>();
    }

    [Fact]
    public async Task Add_And_Delete_Time_Zone_Db_Provider()
    {
        IServiceScope scope = _factory.CreateScope();
        IServiceProvider serviceProvider = scope.ServiceProvider;
        IApplicationTimeRepository<TimeZoneDbProvider> repository =
            serviceProvider.GetRequiredService<IApplicationTimeRepository<TimeZoneDbProvider>>();

        string token = "N321321DSDSADASH10AEDSADSAHP";
        TimeZoneDbToken id = TimeZoneDbToken.Create(token).Value;
        TimeZoneDbProvider provider = new TimeZoneDbProvider(id);

        Result saving = await repository.Add(provider);
        Assert.True(saving.IsSuccess);

        Result<TimeZoneDbProvider> fromDb = await repository.Get();
        Assert.True(fromDb.IsSuccess);
        Assert.Equal(token, fromDb.Value.Id.Id);

        Result deleting = await repository.Remove(token);
        Assert.True(deleting.IsSuccess);
    }

    [Fact]
    public async Task List_Application_Time_Zones()
    {
        string token = "N75LSH9ABRHP";
        TimeZoneDbToken id = TimeZoneDbToken.Create(token).Value;
        TimeZoneDbProvider provider = new(id);

        Result<IReadOnlyList<ApplicationTimeZone>> zones = await provider.ProvideTimeZones();
        Assert.True(zones.IsSuccess);
    }

    [Fact]
    public async Task List_Application_Time_Zones_And_Save()
    {
        IServiceScope scope = _factory.CreateScope();
        IServiceProvider serviceProvider = scope.ServiceProvider;
        IApplicationTimeRepository<TimeZoneDbProvider> repository =
            serviceProvider.GetRequiredService<IApplicationTimeRepository<TimeZoneDbProvider>>();

        string token = "N75LSH9ABRHP";
        TimeZoneDbToken id = TimeZoneDbToken.Create(token).Value;
        TimeZoneDbProvider provider = new TimeZoneDbProvider(id);

        Result saving = await repository.Add(provider);
        Assert.True(saving.IsSuccess);

        Result<IReadOnlyList<ApplicationTimeZone>> zones = await provider.ProvideTimeZones();
        Assert.True(zones.IsSuccess);

        await repository.Save(provider);

        Result<TimeZoneDbProvider> fromDb = await repository.Get();
        Assert.True(fromDb.IsSuccess);
        Assert.Equal(token, fromDb.Value.Id.Id);
        Assert.NotEmpty(fromDb.Value.TimeZones);

        Result deleting = await repository.Remove(token);
        Assert.True(deleting.IsSuccess);
    }

    [Fact]
    public async Task List_Application_Time_Zones_And_Save_Two_Times()
    {
        IServiceScope scope = _factory.CreateScope();
        IServiceProvider provder = scope.ServiceProvider;
        IApplicationTimeRepository<TimeZoneDbProvider> repository = provder.GetRequiredService<
            IApplicationTimeRepository<TimeZoneDbProvider>
        >();

        string token = "N75LSH9ABRHP";
        TimeZoneDbToken id = TimeZoneDbToken.Create(token).Value;
        TimeZoneDbProvider provider = new TimeZoneDbProvider(id);

        Result saving = await repository.Add(provider);
        Assert.True(saving.IsSuccess);

        Result<IReadOnlyList<ApplicationTimeZone>> zones = await provider.ProvideTimeZones();
        Assert.True(zones.IsSuccess);

        await repository.Save(provider);

        Result<TimeZoneDbProvider> fromDb = await repository.Get();
        Assert.True(fromDb.IsSuccess);
        Assert.Equal(token, fromDb.Value.Id.Id);
        Assert.NotEmpty(fromDb.Value.TimeZones);

        int count = fromDb.Value.TimeZones.Count;

        await provider.ProvideTimeZones();
        await repository.Save(provider);

        Result<TimeZoneDbProvider> fromDb2 = await repository.Get();
        Assert.True(fromDb2.IsSuccess);
        Assert.Equal(token, fromDb2.Value.Id.Id);
        Assert.NotEmpty(fromDb2.Value.TimeZones);
        Assert.Equal(count, fromDb2.Value.TimeZones.Count);

        Result deleting = await repository.Remove(token);
        Assert.True(deleting.IsSuccess);
    }
}
