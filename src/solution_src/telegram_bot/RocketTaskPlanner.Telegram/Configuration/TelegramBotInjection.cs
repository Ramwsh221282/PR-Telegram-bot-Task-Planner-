using PRTelegramBot.Core;
using PRTelegramBot.Extensions;
using RocketTaskPlanner.Telegram.TelegramBotFactories;
using Telegram.Bot;

namespace RocketTaskPlanner.Telegram.Configuration;

/// <summary>
/// Utility класс для инъекции зависимостей telegram бота
/// </summary>
public static class TelegramBotInjection
{
    public static void InjectTelegramBot(this HostApplicationBuilder builder)
    {
        BotConfigurationOptions options = builder.Environment.IsDevelopment() switch
        {
            true => BotOptionsResolver.ReadFromEnvironmentVariablesForDev(
                BotConfigurationVariables.EnvFilePath
            ),
            false => BotOptionsResolver.ReadFromEnvironmentVariablesForProd(),
        };
        builder.Services.AddSingleton(options);
        builder.Services.AddSingleton(new TelegramBotClientFactory(options));
        builder.Services.AddTransient<TelegramBotClient>(_ =>
        {
            TelegramBotClient client = new(options.Token);
            return client;
        });
        builder.Services.AddScopedBotHandlers();
    }

    public static PRBotBase GetBotInstance(this IHost host)
    {
        BotConfigurationOptions options =
            host.Services.GetRequiredService<BotConfigurationOptions>();
        IServiceProvider provider = host.Services.GetRequiredService<IServiceProvider>();
        PRBotBase botInstance = new PRBotBuilder(options.Token)
            .SetClearUpdatesOnStart(true)
            .SetServiceProvider(provider)
            .Build();
        return botInstance;
    }
}
