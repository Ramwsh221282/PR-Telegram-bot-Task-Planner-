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
using RocketTaskPlanner.Infrastructure.TimeZoneDb;
using Serilog;

namespace RocketTaskPlanner.Presenters.DependencyInjection;

public static class InjectDependencies
{
    public static IServiceCollection Inject(this IServiceCollection services)
    {
        services.InjectSqlite();
        services.InjectTimeZoneDb();
        services.InjectLogger();
        services.InjectValidators();
        services.InjectUseCases();
        services.InjectQueries();

        RegisterEntityMappings();

        return services;
    }

    private static IServiceCollection InjectLogger(this IServiceCollection services)
    {
        services.AddSingleton<Serilog.ILogger>(
            new LoggerConfiguration().WriteTo.Console().WriteTo.Debug().CreateLogger()
        );

        return services;
    }

    private static IServiceCollection InjectSqlite(this IServiceCollection services)
    {
        services.AddDbContextFactory<ApplicationTimeDbContext>();
        services.AddDbContextFactory<NotificationContextDbContext>();
        services.AddDbContextFactory<PermissionsDbContext>();
        services.AddScoped<ApplicationTimeDbContext>();
        services.AddScoped<NotificationContextDbContext>();
        services.AddScoped<PermissionsDbContext>();

        services.AddTransient<
            INotificationReceiverRepository,
            NotificationReceiverSqliteRepository
        >();

        services.AddSingleton<TimeZoneDbProviderCachedInstance>();
        services.AddTransient<TimeZoneDbCachedRepository>();
        services.AddTransient<
            IApplicationTimeRepository<TimeZoneDbProvider>,
            TimeZoneDbRepository
        >();
        services.AddTransient<IPermissionsRepository, PermissionsRepository>();
        services.Decorate<
            IApplicationTimeRepository<TimeZoneDbProvider>,
            TimeZoneDbCachedRepository
        >();
        return services;
    }

    private static IServiceCollection InjectTimeZoneDb(this IServiceCollection services)
    {
        services.AddSingleton<IApplicationTimeProviderIdFactory, TimeZoneDbTokenFactory>();
        services.AddSingleton<IApplicationTimeProviderFactory, TimeZoneDbInstanceFactory>();
        services.AddTransient<
            IUseCaseHandler<SaveTimeZoneDbApiKeyUseCase, TimeZoneDbProvider>,
            SaveTimeZoneDbApiKeyUseCaseHandler<TimeZoneDbProvider>
        >();

        return services;
    }

    private static IServiceCollection InjectValidators(this IServiceCollection services)
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        services.Scan(scan =>
            scan.FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(IValidator<>)))
                .AsSelfWithInterfaces()
                .WithTransientLifetime()
        );

        return services;
    }

    private static IServiceCollection InjectUseCases(this IServiceCollection services)
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

        return services;
    }

    private static IServiceCollection InjectQueries(this IServiceCollection services)
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

        services.AddSingleton<IDbConnectionFactory, SqliteConnectionFactory>();

        services.Scan(scan =>
            scan.FromAssemblies(assemblies)
                .AddClasses(classes => classes.AssignableTo(typeof(IQueryHandler<,>)))
                .AsSelfWithInterfaces()
                .WithTransientLifetime()
        );

        return services;
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
        });
    }
}
