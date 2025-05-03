using System.Data;
using CSharpFunctionalExtensions;
using Dapper;
using RocketTaskPlanner.Application.ApplicationTimeContext.Repository;
using RocketTaskPlanner.Domain.ApplicationTimeContext.Entities.TimeZones;
using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Entities;
using RocketTaskPlanner.Infrastructure.TimeZoneDb;

namespace RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.GetNotificationReceiverTimeInformation;

/// <summary>
/// Обработчик запроса на получение информации об основном чате по Id с обновленным временем из Time Zone Db провайдера
/// </summary>
/// <param name="factory">Фабрика соединения</param>
/// <param name="repository">Контракт взаимодействия с БД временных зон</param>
public sealed class GetNotificationReceiverTimeInformationQueryHandler(
    IDbConnectionFactory factory,
    IApplicationTimeRepository<TimeZoneDbProvider> repository
)
    : IQueryHandler<
        GetNotificationReceiverTimeInformationQuery,
        GetNotificationReceiverTimeInformationQueryResponse
    >
{
    private const string Sql = """
        SELECT receiver_id, receiver_name, receiver_zone_time_stamp, receiver_zone_name
        FROM notification_receivers
        WHERE receiver_id = @id
        """;

    private readonly IDbConnectionFactory _factory = factory;
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
        using IDbConnection connection = _factory.Create(
            SqliteConstants.NotificationsConnectionString
        );
        return await connection.QueryFirstOrDefaultAsync<NotificationReceiverEntity>(command);
    }

    private static GetNotificationReceiverTimeInformationQueryResponse ManageResponse(
        TimeZoneDbProvider provider,
        NotificationReceiverEntity entity
    )
    {
        string zoneName = entity.ReceiverZoneName;
        ApplicationTimeZone? timeZone = provider.TimeZones.FirstOrDefault(z =>
            z.Name.Name == zoneName
        );
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
            ReceiverZoneTimeStamp = timeZone.TimeInfo.TimeStamp,
        };

        return new GetNotificationReceiverTimeInformationQueryResponse(
            timeZone.TimeInfo.GetDateString(),
            copy,
            timeZone
        );
    }
}
