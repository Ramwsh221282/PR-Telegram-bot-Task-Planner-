using PRTelegramBot.Core;
using PRTelegramBot.Extensions;
using RocketTaskPlanner.Presenters.DependencyInjection;
using RocketTaskPlanner.Telegram.ApplicationNotificationFireService;
using RocketTaskPlanner.Telegram.ApplicationTimeZonesService;
using RocketTaskPlanner.Telegram.Configuration;
using RocketTaskPlanner.Telegram.DatabaseSetup;
using RocketTaskPlanner.Telegram.PermissionsSetup;
using RocketTaskPlanner.Telegram.TelegramBotFactories;
using RocketTaskPlanner.TimeRecognitionModule.TimeCalculation;
using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Facade;
using Telegram.Bot;

if (!Directory.Exists(BotConfigurationVariables.ConfigurationFolder))
    Directory.CreateDirectory(BotConfigurationVariables.ConfigurationFolder);

if (!File.Exists(BotConfigurationVariables.TelegramBotTokenConfigPath))
    throw new FileNotFoundException("File with telegram bot token was not found. Can't start.");

BotConfigurationOptions options = BotOptionsResolver.LoadTgBotOptions(
    BotConfigurationVariables.TelegramBotTokenConfigPath
);

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.AddSingleton(new TelegramBotClientFactory(options));
builder.Services.Inject();
builder.Services.AddTransientBotHandlers();
builder.Services.AddHostedService<ApplicationTimeZonesUpdateService>();
builder.Services.AddHostedService<ApplicationTimeZoneMonitoringService>();
builder.Services.AddHostedService<NotificationsFireService>();
builder.Services.AddTransient<TelegramBotClient>(_ =>
{
    TelegramBotClient client = new(options.Token);
    return client;
});

IHost host = builder.Build();
host.ApplyMigrations().Wait();
host.RegisterBasicPermissions().Wait();
IServiceProvider provider = host.Services.GetRequiredService<IServiceProvider>();
PRBotBase botInstance = new PRBotBuilder(options.Token)
    .SetClearUpdatesOnStart(true)
    .SetServiceProvider(provider)
    .Build();

await botInstance.Start();
Console.WriteLine("Bot started");
host.Run();
