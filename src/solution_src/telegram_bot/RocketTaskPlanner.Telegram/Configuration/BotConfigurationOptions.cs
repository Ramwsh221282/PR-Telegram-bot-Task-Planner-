using RocketTaskPlanner.Infrastructure.Env;

namespace RocketTaskPlanner.Telegram.Configuration;

public sealed record BotConfigurationOptions(string Token);

public static class BotOptionsResolver
{
    public static BotConfigurationOptions ReadFromEnvironmentVariablesForProd()
    {
        IEnvReader reader = new SystemEnvReader();
        string token = reader.GetBotToken();
        return new BotConfigurationOptions(token);
    }

    public static BotConfigurationOptions ReadFromEnvironmentVariablesForDev(string filePath)
    {
        IEnvReader reader = new FileEnvReader(filePath);
        string token = reader.GetBotToken();
        return new BotConfigurationOptions(token);
    }

    private static string GetBotToken(this IEnvReader reader)
    {
        string token = reader.GetEnvironmentVariable("BOT_TOKEN");
        return token;
    }
}
