using System.Data;
using CSharpFunctionalExtensions;
using Dapper;
using RocketTaskPlanner.Application.NotificationsContext.Repository;
using RocketTaskPlanner.Application.Shared.UnitOfWorks;
using RocketTaskPlanner.Domain.NotificationsContext;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverThemes;
using RocketTaskPlanner.Infrastructure.Abstractions;

namespace RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Repositories;

/// <summary>
/// Абстракция для работы с БД чатов, тем чатов и их сообщений
/// </summary>
public sealed class NotificationsWritableRepository(IDbConnectionFactory factory)
    : INotificationsWritableRepository
{
    private readonly IDbConnectionFactory _factory = factory;

    public async Task<Result> Add(
        NotificationReceiver receiver,
        IUnitOfWork unitOfWork,
        CancellationToken ct = default
    )
    {
        const string duplicateSql = """
            SELECT COUNT(*) FROM notification_receivers WHERE receiver_id = @receiver_id
            """;

        const string addSql = """
            INSERT INTO notification_receivers (receiver_id, receiver_name, receiver_zone_name)
            VALUES (@receiver_id, @receiver_name, @receiver_zone_name)
            """;

        using IDbConnection connection = CreateConnection();
        int count = await connection.ExecuteScalarAsync<int>(
            duplicateSql,
            new { receiver_id = receiver.Id.Id }
        );
        if (count != 0)
            return Result.Failure($"Чат с ID: {receiver.Id} уже подписан.");

        var parameters = new
        {
            receiver_id = receiver.Id.Id,
            receiver_name = receiver.Name.Name,
            receiver_zone_name = receiver.TimeZone.ZoneName,
        };

        CommandDefinition command = new(addSql, parameters, cancellationToken: ct);
        UnitOfWorkCommand unitCommand = new(async con => await con.ExecuteAsync(command));
        unitOfWork.AddCommand(this, unitCommand);

        return Result.Success();
    }

    public Result AddTheme(
        ReceiverTheme theme,
        IUnitOfWork unitOfWork,
        CancellationToken ct = default
    )
    {
        const string sql = """
            INSERT INTO receiver_themes (theme_id, receiver_id)
            VALUES (@theme_id, @receiver_id)
            """;

        var parameters = new
        {
            theme_id = theme.Id.Id,
            receiver_id = theme.NotificationReceiverId.Id,
        };

        CommandDefinition command = new(sql, parameters, cancellationToken: ct);
        UnitOfWorkCommand unitCommand = new(async con => await con.ExecuteAsync(command));
        unitOfWork.AddCommand(this, unitCommand);

        return Result.Success();
    }

    public async Task<Result> AddSubject(ThemeChatSubject subject, CancellationToken ct = default)
    {
        const string sql = """
            INSERT INTO theme_chat_subjects (theme_chat_subject_id, theme_id, subject_periodic, subject_created, subject_notify, subject_message)
            VALUES (@theme_chat_subject_id, @theme_id, @subject_periodic, @subject_created, @subject_notify, @subject_message)
            """;

        var parameters = new
        {
            theme_chat_subject_id = subject.Id.Id,
            theme_id = subject.ThemeId.Id,
            subject_periodic = subject.Period.IsPeriodic,
            subject_created = subject.TimeInfo.Created.Value,
            subject_notify = subject.TimeInfo.Notify.Value,
            subject_message = subject.Message.Message,
        };

        CommandDefinition command = new(sql, parameters, cancellationToken: ct);
        using IDbConnection connection = CreateConnection();
        await connection.ExecuteAsync(command);

        return Result.Success();
    }

    public async Task<Result> AddSubject(
        GeneralChatReceiverSubject subject,
        CancellationToken ct = default
    )
    {
        const string sql = """
            INSERT INTO general_chat_subjects (general_chat_subject_id, general_chat_id, subject_periodic, subject_created, subject_notify, subject_message)
            VALUES (@general_chat_subject_id, @general_chat_id, @subject_periodic, @subject_created, @subject_notify, @subject_message)
            """;

        var parameters = new
        {
            general_chat_subject_id = subject.Id.Id,
            general_chat_id = subject.GeneralChatId.Id,
            subject_periodic = subject.Period.IsPeriodic,
            subject_created = subject.TimeInfo.Created.Value,
            subject_notify = subject.TimeInfo.Notify.Value,
            subject_message = subject.Message.Message,
        };
        CommandDefinition command = new(sql, parameters, cancellationToken: ct);
        using IDbConnection connection = CreateConnection();
        await connection.ExecuteAsync(command);

        return Result.Success();
    }

    public Result RemoveTheme(
        ReceiverTheme theme,
        IUnitOfWork unitOfWork,
        CancellationToken ct = default
    )
    {
        const string sql = "DELETE FROM receiver_themes WHERE theme_id = @id";
        var parameters = new { id = theme.Id.Id };

        CommandDefinition command = new(sql, parameters, cancellationToken: ct);
        UnitOfWorkCommand unitCommand = new(async con => await con.ExecuteAsync(command));
        unitOfWork.AddCommand(this, unitCommand);

        return Result.Success();
    }

    public Result Remove(long? id, IUnitOfWork unitOfWork, CancellationToken ct = default)
    {
        const string sql = "DELETE FROM notification_receivers WHERE receiver_id = @id";

        if (id == null)
            return Result.Failure("Чат не был найден");

        var parameters = new { id = id.Value };
        CommandDefinition command = new(sql, parameters, cancellationToken: ct);
        UnitOfWorkCommand unitCommand = new(async con => await con.ExecuteAsync(command));
        unitOfWork.AddCommand(this, unitCommand);

        return Result.Success();
    }

    public IDbConnection CreateConnection() =>
        _factory.Create(SqliteConstants.NotificationsConnectionString);
}
