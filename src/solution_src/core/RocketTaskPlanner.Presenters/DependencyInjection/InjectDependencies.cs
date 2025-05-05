using System.Reflection;
using Dapper.FluentMap;
using Microsoft.Extensions.DependencyInjection;
using RocketTaskPlanner.Application.ApplicationTimeContext.Features.Contracts;
using RocketTaskPlanner.Application.ApplicationTimeContext.Features.SaveTimeZoneDbApiKey;
using RocketTaskPlanner.Application.ApplicationTimeContext.Repository;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Repository;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Visitors;
using RocketTaskPlanner.Application.Facades;
using RocketTaskPlanner.Application.NotificationsContext.Repository;
using RocketTaskPlanner.Application.NotificationsContext.Visitor;
using RocketTaskPlanner.Application.Shared.UnitOfWorks;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Application.Shared.UseCaseHandler.Decorators;
using RocketTaskPlanner.Application.Shared.Validation;
using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Sqlite.ApplicationTimeContext;
using RocketTaskPlanner.Infrastructure.Sqlite.ApplicationTimeContext.Cache;
using RocketTaskPlanner.Infrastructure.Sqlite.ApplicationTimeContext.Entities.EntityMappings;
using RocketTaskPlanner.Infrastructure.Sqlite.ApplicationTimeContext.Repositories;
using RocketTaskPlanner.Infrastructure.Sqlite.DbConnectionFactory;
using RocketTaskPlanner.Infrastructure.Sqlite.ExternalChatsManagementContext;
using RocketTaskPlanner.Infrastructure.Sqlite.ExternalChatsManagementContext.Entities;
using RocketTaskPlanner.Infrastructure.Sqlite.ExternalChatsManagementContext.Entities.EntityMappings;
using RocketTaskPlanner.Infrastructure.Sqlite.ExternalChatsManagementContext.Repositories;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Entities.EntityMappings;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Repositories;
using RocketTaskPlanner.Infrastructure.Sqlite.UnitOfWorks;
using RocketTaskPlanner.Infrastructure.TimeZoneDb;
using RocketTaskPlanner.TimeRecognitionModule.TimeCalculation;
using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Facade;
using Serilog;

namespace RocketTaskPlanner.Presenters.DependencyInjection;

public static class InjectDependencies
{
    // инъекция зависимостей приложения (из core и time_recognition)
    public static void InjectApplicationDependencies(this IServiceCollection services)
    {
        services.InjectSqlite();
        services.InjectTimeZoneDb();
        services.InjectLogger();
        services.InjectValidators();
        services.InjectUseCases();
        services.InjectQueries();
        services.InjectRepositories();
        services.InjectCaches();
        services.InjectTimeRecognition();
        services.InjectUseCaseVisitors();
        services.InjectFacades();
        RegisterEntityMappings();
    }

    // инъекция логгера
    private static void InjectLogger(this IServiceCollection services)
    {
        services.AddSingleton<ILogger>(
            new LoggerConfiguration().WriteTo.Console().WriteTo.Debug().CreateLogger()
        );
    }

    // инъекция зависимостей EntityFramework Core для работы с БД.
    private static void InjectSqlite(this IServiceCollection services)
    {
        services.AddDbContextFactory<ApplicationTimeDbContext>();
        services.AddDbContextFactory<NotificationsDbContext>();
        services.AddDbContextFactory<ExternalChatsDbContext>();

        services.AddSingleton<IDbConnectionFactory, SqliteConnectionFactory>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<ApplicationTimeDbContext>();
        services.AddScoped<NotificationsDbContext>();
        services.AddScoped<ExternalChatsDbContext>();
    }

    // Инъекция контрактов и реализаций для работы с хранилищами (БД).
    private static void InjectRepositories(this IServiceCollection services)
    {
        services.AddTransient<INotificationsWritableRepository, NotificationsWritableRepository>();
        services.AddScoped<INotificationsReadableRepository, NotificationsReadableRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();

        services.AddTransient<TimeZoneDbCachedRepository>();
        services.AddTransient<
            IApplicationTimeRepository<TimeZoneDbProvider>,
            TimeZoneDbRepository
        >();
        services.Decorate<
            IApplicationTimeRepository<TimeZoneDbProvider>,
            TimeZoneDbCachedRepository
        >();

        services.AddScoped<IExternalChatsRepository, ExternalChatsRepository>();
        services.AddScoped<IExternalChatsReadableRepository, ExternalChatsReadableRepository>();
        services.AddScoped<IExternalChatsWritableRepository, ExternalChatsWritableRepository>();
    }

    // инъекция кешей
    private static void InjectCaches(this IServiceCollection services) =>
        services.AddSingleton<TimeZoneDbProviderCachedInstance>();

    // инъекция зависимостей Time Zone Db провайдера
    private static void InjectTimeZoneDb(this IServiceCollection services)
    {
        services.AddSingleton<IApplicationTimeProviderIdFactory, TimeZoneDbTokenFactory>();
        services.AddSingleton<IApplicationTimeProviderFactory, TimeZoneDbInstanceFactory>();
        services.AddTransient<
            IUseCaseHandler<SaveTimeZoneDbApiKeyUseCase, TimeZoneDbProvider>,
            SaveTimeZoneDbApiKeyUseCaseHandler<TimeZoneDbProvider>
        >();
    }

    // инъекция валидации
    private static void InjectValidators(this IServiceCollection services)
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        services.Scan(scan =>
            scan.FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(IValidator<>)))
                .AsSelfWithInterfaces()
                .WithTransientLifetime()
        );
    }

    // инъекция обработчиков бизнес-логики
    private static void InjectUseCases(this IServiceCollection services)
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        services.Scan(scan =>
            scan.FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(IUseCaseHandler<,>)))
                .AsSelfWithInterfaces()
                .WithScopedLifetime()
        );

        services.Decorate(typeof(IUseCaseHandler<,>), typeof(GenericValidationDecorator<,>));
        services.Decorate(
            typeof(IUseCaseHandler<,>),
            typeof(GenericExceptionSupressingDecorator<,>)
        );
        services.Decorate(typeof(IUseCaseHandler<,>), typeof(GenericLoggingDecorator<,>));
        services.Decorate(typeof(IUseCaseHandler<,>), typeof(GenericMetricsHandlerDecorator<,>));
    }

    // инъекция обработчиков запросов
    private static void InjectQueries(this IServiceCollection services)
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        services.Scan(scan =>
            scan.FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
                .AsSelfWithInterfaces()
                .WithScopedLifetime()
        );
    }

    // инъекция логики для распознавания времени в тексте
    private static void InjectTimeRecognition(this IServiceCollection services)
    {
        services.AddSingleton<TimeRecognitionFacade>();
        services.AddSingleton<TimeCalculationService>();
    }

    // регистрация маппинга из моделей таблиц в Dao модели приложения при запросах через Dapper
    private static void RegisterEntityMappings()
    {
        FluentMapper.Initialize(config =>
        {
            config.AddMap(new GeneralChatSubjectEntityMap());
            config.AddMap(new NotificationReceiverEntityMap());
            config.AddMap(new ReceiverThemeEntityMap());
            config.AddMap(new ThemeChatSubjectEntityMap());
            config.AddMap(new TimeZoneDbProviderEntityMap());
            config.AddMap(new TimeZoneEntityMap());
            config.AddMap(new ExternalChatEntityMap());
            config.AddMap(new ExternalChatOwnerMap());
        });
    }

    // инъекция посетителей запросов бизнес-логики
    private static void InjectUseCaseVisitors(this IServiceCollection services)
    {
        services.AddScoped<INotificationUseCaseVisitor, NotificationUseCaseVisitor>();
        services.AddScoped<IExternalChatUseCasesVisitor, ExternalChatUseCasesVisitor>();
    }

    private static void InjectFacades(this IServiceCollection services)
    {
        services.AddScoped<FirstUserChatRegistrationFacade>();
        services.AddScoped<UserChatRegistrationFacade>();
        services.AddScoped<UserThemeRegistrationFacade>();
        services.AddScoped<RemoveThemeChatFacade>();
        services.AddScoped<RemoveOwnerChatFacade>();
    }
}
