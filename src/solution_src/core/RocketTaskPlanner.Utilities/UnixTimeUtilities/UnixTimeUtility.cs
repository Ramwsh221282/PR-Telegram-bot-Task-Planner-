namespace RocketTaskPlanner.Utilities.UnixTimeUtilities;

/// <summary>
/// Extension методы преобразований даты из Unix в Date Time и обратно.
/// </summary>
public static class UnixTimeUtility
{
    /// <summary>
    /// Преобразование даты из Date Time в Unix (в секундах)
    /// </summary>
    /// <param name="dateTime">DateTime</param>
    /// <returns>
    /// Unix время в секундах
    /// </returns>
    public static long ToUnixTimeSeconds(this DateTime dateTime)
    {
        DateTime utcDateTime = dateTime.ToUniversalTime();
        DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return (long)(utcDateTime - unixEpoch).TotalSeconds;
    }

    /// <summary>
    /// Преобразование даты из Unix секунд в DateTime
    /// </summary>
    /// <param name="unixTimeSeconds">Unix секунды</param>
    /// <returns>
    ///     <inheritdoc cref="DateTime"/>
    /// </returns>
    public static DateTime FromUnixTimeSeconds(this long unixTimeSeconds)
    {
        DateTimeOffset offset = DateTimeOffset.FromUnixTimeSeconds(unixTimeSeconds);
        return offset.DateTime;
    }
}
