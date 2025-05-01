namespace RocketTaskPlanner.Utilities.DateExtensions;

public static class DateTimeExtensions
{
    public static string AsString(this DateTime date) => date.ToString("HH:mm:ss dd/MM/yyyy");
}
