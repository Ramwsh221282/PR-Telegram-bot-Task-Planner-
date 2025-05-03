using PRTelegramBot.Core;
using PRTelegramBot.Extensions;
using RocketTaskPlanner.Presenters.DependencyInjection;
using RocketTaskPlanner.Telegram.ApplicationNotificationFireService;
using RocketTaskPlanner.Telegram.ApplicationTimeZonesService;
using RocketTaskPlanner.Telegram.Configuration;
using RocketTaskPlanner.Telegram.DatabaseSetup;
using RocketTaskPlanner.Telegram.PermissionsSetup;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);

builder.Services.InjectApplicationDependencies(); // инъекция всех зависимостей приложения, не связанных с ботом.
builder.Services.AddTransientBotHandlers(); // инъекция зависимостей бота как transient.
builder.InjectTelegramBot(); // инъекция зависимостей бота.

builder.Services.AddHostedService<ApplicationTimeZonesUpdateService>(); // инъекция сервиса для обновления кеша временных зон.
builder.Services.AddHostedService<ApplicationTimeZoneMonitoringService>(); // инъекция сервиса для мониторинга временных зон.
builder.Services.AddHostedService<NotificationsFireService>(); // инъекция сервиса для уведомлений.

IHost host = builder.Build();

host.ApplyMigrations().Wait(); // применение миграций БД.
host.RegisterBasicPermissions().Wait(); // добавление базовых прав (edit, create tasks).

PRBotBase bot = host.GetBotInstance();
await bot.Start(); // запуск бота.
Console.WriteLine("Bot started");
host.Run(); // запуск приложения.
