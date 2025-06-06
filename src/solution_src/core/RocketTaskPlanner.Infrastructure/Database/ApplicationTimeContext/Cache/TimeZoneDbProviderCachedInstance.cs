using RocketTaskPlanner.Infrastructure.TimeZoneDb;

namespace RocketTaskPlanner.Infrastructure.Database.ApplicationTimeContext.Cache;

/// <summary>
/// Кеш для хранения экземпляра провайдера временных зон.
/// </summary>
public sealed class TimeZoneDbProviderCachedInstance
{
    private TimeZoneDbProvider? _instance = null;
    public TimeZoneDbProvider? Instance => _instance;

    public void InitializeOrUpdate(TimeZoneDbProvider instance)
    {
        _instance = instance;
    }

    public void SetNull()
    {
        _instance = null;
    }
}
