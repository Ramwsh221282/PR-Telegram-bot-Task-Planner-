using PRTelegramBot.Core;
using PRTelegramBot.Extensions;
using RocketTaskPlanner.Presenters.DependencyInjection;
using RocketTaskPlanner.Telegram.ApplicationNotificationFireService;
using RocketTaskPlanner.Telegram.ApplicationTimeZonesService;
using RocketTaskPlanner.Telegram.Configuration;
using RocketTaskPlanner.Telegram.DatabaseSetup;
using RocketTaskPlanner.Telegram.PermissionsSetup;

HostApplicationBuilder builder = Host.CreateApplicationBuilder(args);
builder.Services.InjectApplicationDependencies();
builder.Services.AddTransientBotHandlers();
builder.InjectTelegramBot();

builder.Services.AddHostedService<ApplicationTimeZonesUpdateService>();
builder.Services.AddHostedService<ApplicationTimeZoneMonitoringService>();
builder.Services.AddHostedService<NotificationsFireService>();

IHost host = builder.Build();

host.ApplyMigrations().Wait();
host.RegisterBasicPermissions().Wait();
PRBotBase bot = host.GetBotInstance();
await bot.Start();
Console.WriteLine("Bot started");
host.Run();
