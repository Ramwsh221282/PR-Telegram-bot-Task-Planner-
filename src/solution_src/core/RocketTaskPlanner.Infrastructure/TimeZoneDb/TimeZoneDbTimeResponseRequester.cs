using System.Text.Json;
using CSharpFunctionalExtensions;

namespace RocketTaskPlanner.Infrastructure.TimeZoneDb;

/// <summary>
/// Запрашиватель временных зон по HTTP
/// <param name="provider">
///     <inheritdoc cref="TimeZoneDbProvider"/>
/// </param>
/// </summary>
public sealed class TimeZoneDbTimeResponseRequester(TimeZoneDbProvider provider)
{
    /// <summary>
    /// <inheritdoc cref="TimeZoneDbProvider"/>
    /// </summary>
    private readonly TimeZoneDbProvider _provider = provider;

    /// <summary>
    /// Возврат ответа от сервиса. Ответ - список временных зон
    /// <returns>
    /// <inheritdoc cref="TimeZoneDbTimeResponse"/>
    /// </returns>
    /// </summary>
    public async Task<Result<TimeZoneDbTimeResponse[]>> GetResponse()
    {
        using HttpClient client = new();
        try
        {
            string requestUrl = CreateRequestUrl();
            HttpResponseMessage responseMessage = await client.GetAsync(requestUrl);
            await Task.Delay(TimeSpan.FromSeconds(3)); // free API limit.
            if (!responseMessage.IsSuccessStatusCode)
            {
                string content = await responseMessage.Content.ReadAsStringAsync();
                return Result.Failure<TimeZoneDbTimeResponse[]>(content);
            }

            string json = await responseMessage.Content.ReadAsStringAsync();
            return ParseJson(json);
        }
        catch
        {
            const string content = "Произошла внутренняя ошибка при запросе в Time Zone Db";
            return Result.Failure<TimeZoneDbTimeResponse[]>(content);
        }
    }

    /// <summary>
    /// Парсинг json ответа от Time Zone Db сервиса
    /// <param name="json">Json ответ от запроса в Time Zone Db сервис</param>
    /// <returns>
    ///     <inheritdoc cref="TimeZoneDbTimeResponse"/>
    /// </returns>
    /// </summary>
    private static Result<TimeZoneDbTimeResponse[]> ParseJson(string json)
    {
        using JsonDocument document = JsonDocument.Parse(json);
        try
        {
            if (!document.RootElement.TryGetProperty("zones", out JsonElement zonesArrayElement))
                return Result.Failure<TimeZoneDbTimeResponse[]>(
                    $"Некорректный ответ от Time Zone Db. Ответ: {json}"
                );

            TimeZoneDbTimeResponse[] times = ParseJsonArray(zonesArrayElement);
            return times;
        }
        catch
        {
            return Result.Failure<TimeZoneDbTimeResponse[]>(
                $"Не удалось распарсить ответ от Time Zone Db. Ответ: {json}"
            );
        }
    }

    private static TimeZoneDbTimeResponse[] ParseJsonArray(JsonElement array)
    {
        int length = array.GetArrayLength();
        TimeZoneDbTimeResponse[] times = new TimeZoneDbTimeResponse[length];
        int lastIndex = 0;

        foreach (JsonElement element in array.EnumerateArray())
        {
            TimeZoneDbTimeResponse response = ParseJsonElement(element);
            times[lastIndex] = response;
            lastIndex++;
        }

        return times;
    }

    private static TimeZoneDbTimeResponse ParseJsonElement(JsonElement element)
    {
        JsonElement timeZoneElement = element.GetProperty("zoneName");
        JsonElement timeStampElement = element.GetProperty("timestamp");

        string zoneName = timeZoneElement.GetString()!;
        long timeStamp = timeStampElement.GetInt64();

        TimeZoneDbTimeResponse response = new(zoneName, timeStamp);

        return response;
    }

    private string CreateRequestUrl() =>
        $"https://api.timezonedb.com/v2.1/list-time-zone&key={_provider.Id.Id}&format=json&country=RU";
}
