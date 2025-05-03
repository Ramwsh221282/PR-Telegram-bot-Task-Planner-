namespace RocketTaskPlanner.Telegram.Configuration;

public static class BotConfigurationVariables
{
    private static readonly string ConfigurationFolder = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory
    );

    /// <summary>
    /// Путь к .env файлу. Файл .env должен находится в папке с файлом запуска.
    /// </summary>
    public static readonly string EnvFilePath = Path.Combine(ConfigurationFolder, ".env");
}
