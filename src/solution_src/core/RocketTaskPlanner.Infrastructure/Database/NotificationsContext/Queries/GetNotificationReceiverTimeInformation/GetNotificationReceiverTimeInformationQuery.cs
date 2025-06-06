using CSharpFunctionalExtensions;
using Dapper;
using RocketTaskPlanner.Application.ApplicationTimeContext.Repository;
using RocketTaskPlanner.Domain.ApplicationTimeContext.Entities.TimeZones;
using RocketTaskPlanner.Domain.NotificationsContext;
using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Database.NotificationsContext.Entities;
using RocketTaskPlanner.Infrastructure.TimeZoneDb;

namespace RocketTaskPlanner.Infrastructure.Database.NotificationsContext.Queries.GetNotificationReceiverTimeInformation;

/// <summary>
/// Запрос на получение информации о времени <inheritdoc cref="NotificationReceiver"/>
/// <param name="ChatId">ID чата</param>
/// </summary>
public sealed record GetNotificationReceiverTimeInformationQuery(long ChatId) : IQuery;

public sealed record GetNotificationReceiverTimeInformationQueryResponse(
    string Information,
    NotificationReceiverEntity? Entity,
    ApplicationTimeZone? TimeZone);

public sealed class GetNotificationReceiverTimeInformationQueryHandler(
    IDbConnectionFactory factory, IApplicationTimeRepository<TimeZoneDbProvider> repository
)
    : IQueryHandler<
        GetNotificationReceiverTimeInformationQuery,
        GetNotificationReceiverTimeInformationQueryResponse
    >
{
    /// <summary>
    /// SQL запрос
    /// </summary>
    private const string Sql = """
        SELECT receiver_id, receiver_name, receiver_zone_name
        FROM notification_receivers
        WHERE receiver_id = @id
        """;

    /// <summary>
    ///     <inheritdoc cref="IDbConnectionFactory"/>
    /// </summary>
    private readonly IDbConnectionFactory _factory = factory;

    /// <summary>
    ///     <inheritdoc cref="IApplicationTimeRepository{TProvider}"/>
    /// </summary>
    private readonly IApplicationTimeRepository<TimeZoneDbProvider> _repository = repository;

    public async Task<GetNotificationReceiverTimeInformationQueryResponse> Handle(
        GetNotificationReceiverTimeInformationQuery informationQuery,
        CancellationToken ct = default
    )
    {
        Result<TimeZoneDbProvider> provider = await _repository.Get(ct);
        if (provider.IsFailure)
            return new GetNotificationReceiverTimeInformationQueryResponse(
                provider.Error,
                null,
                null
            );

        NotificationReceiverEntity? entity = await QueryReceiver(informationQuery.ChatId, ct);
        return entity == null
            ? new GetNotificationReceiverTimeInformationQueryResponse(
                $"Не удалось найти время чата с ID: {informationQuery.ChatId}. Наверное для чата ещё не было задано время.",
                null,
                null
            )
            : ManageResponse(provider.Value, entity);
    }

    private async Task<NotificationReceiverEntity?> QueryReceiver(long id, CancellationToken token)
    {
        var parameters = new { id };
        CommandDefinition command = new(Sql, parameters, cancellationToken: token);
        using var connection = _factory.Create();
        return await connection.QueryFirstOrDefaultAsync<NotificationReceiverEntity>(command);
    }

    private static GetNotificationReceiverTimeInformationQueryResponse ManageResponse(
        TimeZoneDbProvider provider,
        NotificationReceiverEntity entity
    )
    {
        string zoneName = entity.ReceiverZoneName;
        var timeZone = provider.TimeZones.FirstOrDefault(z => z.Name.Name == zoneName);
        if (timeZone == null)
            return new GetNotificationReceiverTimeInformationQueryResponse(
                $"Не удалось найти время чата с ID: {entity.ReceiverId}. Наверное для чата ещё не было задано время.",
                null,
                null
            );

        NotificationReceiverEntity copy = new()
        {
            ReceiverId = entity.ReceiverId,
            ReceiverName = entity.ReceiverName,
            ReceiverSubjects = entity.ReceiverSubjects,
            ReceiverThemes = entity.ReceiverThemes,
            ReceiverZoneName = entity.ReceiverZoneName,
        };

        return new GetNotificationReceiverTimeInformationQueryResponse(
            timeZone.TimeInfo.GetDateString(),
            copy,
            timeZone
        );
    }
}
