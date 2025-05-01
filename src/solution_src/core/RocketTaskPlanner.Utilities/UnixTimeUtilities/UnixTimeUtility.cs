namespace RocketTaskPlanner.Utilities.UnixTimeUtilities;

public static class UnixTimeUtility
{
    public static long ToUnixTimeSeconds(this DateTime dateTime)
    {
        DateTime utcDateTime = dateTime.ToUniversalTime();
        DateTime unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
        return (long)(utcDateTime - unixEpoch).TotalSeconds;
    }

    public static DateTime FromUnixTimeSeconds(this long unixTimeSeconds)
    {
        DateTimeOffset offset = DateTimeOffset.FromUnixTimeSeconds(unixTimeSeconds);
        return offset.DateTime;
    }
}
