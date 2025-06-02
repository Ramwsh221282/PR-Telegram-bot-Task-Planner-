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
using RocketTaskPlanner.Infrastructure.Database;
using RocketTaskPlanner.Infrastructure.Database.ApplicationTimeContext.Cache;
using RocketTaskPlanner.Infrastructure.Database.ApplicationTimeContext.Entities.EntityMappings;
using RocketTaskPlanner.Infrastructure.Database.ApplicationTimeContext.Repositories;
using RocketTaskPlanner.Infrastructure.Database.DbConnectionFactory;
using RocketTaskPlanner.Infrastructure.Database.ExternalChatsManagementContext.Entities.EntityMappings;
using RocketTaskPlanner.Infrastructure.Database.ExternalChatsManagementContext.Repositories;
using RocketTaskPlanner.Infrastructure.Database.NotificationsContext.Entities.EntityMappings;
using RocketTaskPlanner.Infrastructure.Database.NotificationsContext.Repositories;
using RocketTaskPlanner.Infrastructure.Database.UnitOfWorks;
using RocketTaskPlanner.Infrastructure.SeqConfiguration;
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
    public static void InjectLogger(this IServiceCollection services, SeqConfiguration seq)
    {
        services.AddSingleton<ILogger>(
            new LoggerConfiguration()
                .WriteTo.Console()
                .WriteTo.Debug()
                .WriteTo.Seq(seq.HostName)
                .CreateLogger()
        );
    }

    // инъекция зависимостей EntityFramework Core для работы с БД.
    private static void InjectSqlite(this IServiceCollection services)
    {
        services.AddSingleton<IDbConnectionFactory, PostgresDbConnectionFactory>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<RocketTaskPlannerDbContext>();
    }

    // Инъекция контрактов и реализаций для работы с хранилищами (БД).
    private static void InjectRepositories(this IServiceCollection services)
    {
        services.AddScoped<TimeZoneDbCachedRepository>();
        services.AddScoped<IApplicationTimeRepository<TimeZoneDbProvider>, TimeZoneDbRepository>();
        services.Decorate<
            IApplicationTimeRepository<TimeZoneDbProvider>,
            TimeZoneDbCachedRepository
        >();

        services.AddScoped<INotificationsWritableRepository, NotificationsWritableRepository>();
        services.AddScoped<IExternalChatsWritableRepository, ExternalChatsWritableRepository>();

        services.AddTransient<IExternalChatsReadableRepository, ExternalChatsReadableRepository>();
        services.AddTransient<INotificationsReadableRepository, NotificationsReadableRepository>();
    }

    // инъекция кешей
    private static void InjectCaches(this IServiceCollection services) =>
        services.AddSingleton<TimeZoneDbProviderCachedInstance>();

    // инъекция зависимостей Time Zone Db провайдера
    private static void InjectTimeZoneDb(this IServiceCollection services)
    {
        services.AddSingleton<IApplicationTimeProviderIdFactory, TimeZoneDbTokenFactory>();
        services.AddSingleton<IApplicationTimeProviderFactory, TimeZoneDbInstanceFactory>();
        services.AddScoped<
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
                .WithScopedLifetime()
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
                .WithTransientLifetime()
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
