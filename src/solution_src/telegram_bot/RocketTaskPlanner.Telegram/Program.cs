using PRTelegramBot.Core;
using PRTelegramBot.Extensions;
using RocketTaskPlanner.Infrastructure.Database;
using RocketTaskPlanner.Infrastructure.SeqConfiguration;
using RocketTaskPlanner.Presenters.DependencyInjection;
using RocketTaskPlanner.Telegram.ApplicationNotificationFireService;
using RocketTaskPlanner.Telegram.ApplicationTimeZonesService;
using RocketTaskPlanner.Telegram.Configuration;
using RocketTaskPlanner.Telegram.StartupExtensions.DatabaseSetup;
using RocketTaskPlanner.Telegram.StartupExtensions.TimeZoneDbSetup;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);
SeqConfiguration seq = null!;
if (builder.Environment.IsDevelopment())
{
    DatabaseConfiguration.AddFromEnvFile(builder.Services, BotConfigurationVariables.EnvFilePath);
    seq = SeqConfiguration.FromEnvFile(builder.Services, BotConfigurationVariables.EnvFilePath);
}
else
{
    DatabaseConfiguration.AddFromEnvironmentVariables(builder.Services);
    seq = SeqConfiguration.FromEnvironmentVariables(builder.Services);
}

builder.Services.InjectApplicationDependencies(); // инъекция всех зависимостей приложения, не связанных с ботом.
builder.Services.AddScopedBotHandlers();
builder.InjectTelegramBot(); // инъекция зависимостей бота.

builder.Services.AddHostedService<ApplicationTimeZonesUpdateService>(); // инъекция сервиса для обновления кеша временных зон.
builder.Services.AddHostedService<ApplicationTimeZoneMonitoringService>(); // инъекция сервиса для мониторинга временных зон.
builder.Services.AddHostedService<NotificationsFireService>(); // инъекция сервиса для уведомлений.

IHost host = builder.Build();

host.ApplyMigrations().Wait(); // применение миграций БД.

if (builder.Environment.IsDevelopment())
    host.SetupTimeZoneDbProviderUsingEnvFile(BotConfigurationVariables.EnvFilePath).Wait(); // установка провайдера временных зон из .env файла
else
    host.SetupTimeZoneDbProviderUsingEnvVariables().Wait(); // установка провайдера временных зон из переменных окружения

PRBotBase bot = host.GetBotInstance();
await bot.Start(); // запуск бота.
Console.WriteLine("Bot started");
host.Run(); // запуск приложения.
