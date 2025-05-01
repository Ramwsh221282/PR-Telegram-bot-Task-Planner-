using System.Text.Json;

namespace RocketTaskPlanner.Telegram.Configuration;

public sealed record BotConfigurationOptions(string Token);

public static class BotOptionsResolver
{
    public static BotConfigurationOptions LoadTgBotOptions(string filePath)
    {
        using JsonDocument document = JsonDocument.Parse(File.ReadAllText(filePath));
        document.RootElement.TryGetProperty("Token", out JsonElement token);
        string? tokenValue = token.GetString();
        if (string.IsNullOrWhiteSpace(tokenValue))
            throw new ApplicationException("Некорректный конфигурационный файл телеграм бота");
        return new BotConfigurationOptions(tokenValue);
    }
}
