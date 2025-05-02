namespace RocketTaskPlanner.Telegram.Configuration;

public static class BotConfigurationVariables
{
    private static readonly string ConfigurationFolder = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory
    );

    public static readonly string EnvFilePath = Path.Combine(ConfigurationFolder, ".env");
}
