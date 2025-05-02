using System.Reflection;
using Dapper.FluentMap;
using Microsoft.Extensions.DependencyInjection;
using RocketTaskPlanner.Application.ApplicationTimeContext.Features.SaveTimeZoneDbApiKey;
using RocketTaskPlanner.Application.ApplicationTimeContext.Repository;
using RocketTaskPlanner.Application.NotificationsContext.Repository;
using RocketTaskPlanner.Application.PermissionsContext.Repository;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Application.Shared.UseCaseHandler.Decorators;
using RocketTaskPlanner.Application.Shared.Validation;
using RocketTaskPlanner.Application.UsersContext.Contracts;
using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Sqlite.ApplicationTimeContext;
using RocketTaskPlanner.Infrastructure.Sqlite.ApplicationTimeContext.Cache;
using RocketTaskPlanner.Infrastructure.Sqlite.ApplicationTimeContext.Entities.EntityMappings;
using RocketTaskPlanner.Infrastructure.Sqlite.ApplicationTimeContext.Repositories;
using RocketTaskPlanner.Infrastructure.Sqlite.DbConnectionFactory;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Entities.EntityMappings;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Repositories;
using RocketTaskPlanner.Infrastructure.Sqlite.PermissionsContext;
using RocketTaskPlanner.Infrastructure.Sqlite.PermissionsContext.Entities.EntityMappings;
using RocketTaskPlanner.Infrastructure.Sqlite.UsersContext;
using RocketTaskPlanner.Infrastructure.Sqlite.UsersContext.Entities.EntityMappings;
using RocketTaskPlanner.Infrastructure.TimeZoneDb;
using RocketTaskPlanner.TimeRecognitionModule.TimeCalculation;
using RocketTaskPlanner.TimeRecognitionModule.TimeRecognition.Facade;
using Serilog;

namespace RocketTaskPlanner.Presenters.DependencyInjection;

public static class InjectDependencies
{
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
        RegisterEntityMappings();
    }

    private static void InjectLogger(this IServiceCollection services)
    {
        services.AddSingleton<ILogger>(
            new LoggerConfiguration().WriteTo.Console().WriteTo.Debug().CreateLogger()
        );
    }

    private static void InjectSqlite(this IServiceCollection services)
    {
        services.AddDbContextFactory<ApplicationTimeDbContext>();
        services.AddDbContextFactory<NotificationsDbContext>();
        services.AddDbContextFactory<PermissionsDbContext>();
        services.AddDbContextFactory<UsersDbContext>();

        services.AddScoped<ApplicationTimeDbContext>();
        services.AddScoped<NotificationsDbContext>();
        services.AddScoped<PermissionsDbContext>();
        services.AddScoped<UsersDbContext>();
    }

    private static void InjectRepositories(this IServiceCollection services)
    {
        services.AddTransient<
            INotificationReceiverRepository,
            NotificationReceiverSqliteRepository
        >();
        services.AddTransient<TimeZoneDbCachedRepository>();
        services.AddTransient<
            IApplicationTimeRepository<TimeZoneDbProvider>,
            TimeZoneDbRepository
        >();
        services.Decorate<
            IApplicationTimeRepository<TimeZoneDbProvider>,
            TimeZoneDbCachedRepository
        >();

        services.AddTransient<IPermissionsWritableRepository, PermissionsWritableRepository>();
        services.AddTransient<IPermissionsReadableRepository, PermissionsReadableRepository>();
        services.AddTransient<IPermissionsRepository, PermissionsRepository>();

        services.AddTransient<IUsersReadableRepository, UsersReadableRepository>();
        services.AddTransient<IUsersWritableRepository, UsersWritableRepository>();
        services.AddTransient<IUsersRepository, UsersRepository>();
    }

    private static void InjectCaches(this IServiceCollection services) =>
        services.AddSingleton<TimeZoneDbProviderCachedInstance>();

    private static void InjectTimeZoneDb(this IServiceCollection services)
    {
        services.AddSingleton<IApplicationTimeProviderIdFactory, TimeZoneDbTokenFactory>();
        services.AddSingleton<IApplicationTimeProviderFactory, TimeZoneDbInstanceFactory>();
        services.AddTransient<
            IUseCaseHandler<SaveTimeZoneDbApiKeyUseCase, TimeZoneDbProvider>,
            SaveTimeZoneDbApiKeyUseCaseHandler<TimeZoneDbProvider>
        >();
    }

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

    private static void InjectUseCases(this IServiceCollection services)
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        services.Scan(scan =>
            scan.FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(IUseCaseHandler<,>)))
                .AsSelfWithInterfaces()
                .WithTransientLifetime()
        );

        services.Decorate(typeof(IUseCaseHandler<,>), typeof(GenericValidationDecorator<,>));
        services.Decorate(
            typeof(IUseCaseHandler<,>),
            typeof(GenericExceptionSupressingDecorator<,>)
        );
        services.Decorate(typeof(IUseCaseHandler<,>), typeof(GenericLoggingDecorator<,>));
        services.Decorate(typeof(IUseCaseHandler<,>), typeof(GenericMetricsHandlerDecorator<,>));
    }

    private static void InjectQueries(this IServiceCollection services)
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        services.AddSingleton<IDbConnectionFactory, SqliteConnectionFactory>();

        services.Scan(scan =>
            scan.FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
                .AsSelfWithInterfaces()
                .WithTransientLifetime()
        );
    }

    private static void InjectTimeRecognition(this IServiceCollection services)
    {
        services.AddSingleton<TimeRecognitionFacade>();
        services.AddSingleton<TimeCalculationService>();
    }

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
            config.AddMap(new UserEntityMap());
            config.AddMap(new UserPermissionEntityMap());
            config.AddMap(new PermissionEntityMap());
        });
    }
}
