using RocketTaskPlanner.Infrastructure.Env;

namespace RocketTaskPlanner.Telegram.Configuration;

/// <summary>
/// Настройки телеграм бота, содержащие токен
/// </summary>
/// <param name="Token">Токен телеграм бота из BotFather</param>
public sealed record BotConfigurationOptions(string Token);

/// <summary>
/// Utility класс для чтение настроек бота
/// </summary>
public static class BotOptionsResolver
{
    /// <summary>
    /// Чтение из переменных окружения системы
    /// </summary>
    /// <returns>Конфигурация бота</returns>
    public static BotConfigurationOptions ReadFromEnvironmentVariablesForProd()
    {
        IEnvReader reader = new SystemEnvReader();
        string token = reader.GetBotToken();
        string name = reader.GetBotName();
        return new BotConfigurationOptions(token);
    }

    /// <summary>
    /// Чтение из .env файла.
    /// </summary>
    /// <param name="filePath">Путь к .env файлу с токеном бота</param>
    /// <returns>Конфигурация бота</returns>
    public static BotConfigurationOptions ReadFromEnvironmentVariablesForDev(string filePath)
    {
        IEnvReader reader = new FileEnvReader(filePath);
        string token = reader.GetBotToken();
        string name = reader.GetBotName();
        return new BotConfigurationOptions(token);
    }

    private static string GetBotToken(this IEnvReader reader)
    {
        string token = reader.GetEnvironmentVariable("BOT_TOKEN");
        return token;
    }

    private static string GetBotName(this IEnvReader reader)
    {
        string token = reader.GetEnvironmentVariable("BOT_NAME");
        return token;
    }
}
