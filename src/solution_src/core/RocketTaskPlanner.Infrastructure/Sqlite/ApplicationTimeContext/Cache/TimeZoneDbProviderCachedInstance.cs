using RocketTaskPlanner.Infrastructure.TimeZoneDb;

namespace RocketTaskPlanner.Infrastructure.Sqlite.ApplicationTimeContext.Cache;

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
