using Microsoft.Extensions.DependencyInjection;

namespace RocketTaskPlanner.Tests.TestDependencies;

public sealed class DefaultTestsFixture : IAsyncDisposable, IDisposable
{
    private readonly IServiceScope _scope;
    private bool _isDisposed = false;

    public DefaultTestsFixture()
    {
        IServiceCollection services = TestsServiceCollection.BuildServicesCollection();
        IServiceProvider provider = services.BuildServiceProvider();
        IServiceScopeFactory factory = provider.GetRequiredService<IServiceScopeFactory>();
        IServiceScope scope = factory.CreateScope();
        _scope = scope;
        _scope.SetupDatabases().Wait();
    }

    public void Dispose()
    {
        if (_isDisposed)
            return;
        TestsDatabaseDrop.DropDatabases().Wait();
        _scope.Dispose();
        _isDisposed = true;
    }

    public async ValueTask DisposeAsync()
    {
        if (_isDisposed)
            return;
        await TestsDatabaseDrop.DropDatabases();
        _scope.Dispose();
        _isDisposed = true;
    }

    public T GetService<T>()
        where T : class
    {
        T service = _scope.ServiceProvider.GetRequiredService<T>();
        return service;
    }
}
