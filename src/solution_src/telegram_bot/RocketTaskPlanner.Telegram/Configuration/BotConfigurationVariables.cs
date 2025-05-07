namespace RocketTaskPlanner.Telegram.Configuration;

/// <summary>
/// Константные значения путей конфигурации
/// </summary>
public static class BotConfigurationVariables
{
    /// <summary>
    /// Текущая папка приложения (там где .exe файл или .dll файл Telegram бота)
    /// </summary>
    private static readonly string ConfigurationFolder = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory
    );

    /// <summary>
    /// Путь к .env файлу. Файл .env должен находится в папке с файлом запуска.
    /// </summary>
    public static readonly string EnvFilePath = Path.Combine(ConfigurationFolder, ".env");
}
