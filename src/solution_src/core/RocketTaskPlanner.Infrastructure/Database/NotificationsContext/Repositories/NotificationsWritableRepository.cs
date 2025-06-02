using CSharpFunctionalExtensions;
using Dapper;
using Microsoft.EntityFrameworkCore;
using RocketTaskPlanner.Application.NotificationsContext.Repository;
using RocketTaskPlanner.Domain.NotificationsContext;
using RocketTaskPlanner.Domain.NotificationsContext.ValueObjects;
using RocketTaskPlanner.Infrastructure.Abstractions;

namespace RocketTaskPlanner.Infrastructure.Database.NotificationsContext.Repositories;

/// <summary>
/// Абстракция для работы с <inheritdoc cref="NotificationReceiver"/>
/// </summary>
public sealed class NotificationsWritableRepository(RocketTaskPlannerDbContext context, IDbConnectionFactory connectionFactory)
    : INotificationsWritableRepository, IAsyncDisposable
{
    private readonly RocketTaskPlannerDbContext _context = context;
    private readonly IDbConnectionFactory _connectionFactory = connectionFactory;

    /// <summary>
    /// Добавление <inheritdoc cref="NotificationReceiver"/> в базу данных
    /// <param name="receiver">
    ///     <inheritdoc cref="NotificationReceiver"/>
    /// </param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Result Success или Failure</returns>
    /// </summary>
    public async Task<Result<NotificationReceiver>> Add(
        NotificationReceiver receiver,
        CancellationToken ct = default)
    {
        if (await _context.Receivers.AsNoTracking().AnyAsync(r => r.Id == receiver.Id, cancellationToken: ct))
        {
            return Result.Failure<NotificationReceiver>($"Чат с ID: {receiver.Id} уже подписан.");
        }
        
        await _context.Receivers.AddAsync(receiver, ct);
        return receiver;
    }

    public void Remove(NotificationReceiver receiver) => _context.Receivers.Remove(receiver);

    public async Task<Result<NotificationReceiver>> GetById(long receiverId, CancellationToken ct = default)
    {
        var receiverIdModel = NotificationReceiverId.Create(receiverId);
        if (receiverIdModel.IsFailure) 
            return Result.Failure<NotificationReceiver>($"Не найден чат с ID: {receiverId}");
        
        var receiver = await _context.Receivers
            .Include(r => r.Subjects)
            .Include(r => r.Themes)
            .ThenInclude(st => st.Subjects)
            .FirstOrDefaultAsync(r => r.Id == receiverIdModel.Value, cancellationToken: ct);
        
        return receiver ?? Result.Failure<NotificationReceiver>($"Не найден чат с ID: {receiverId}");
    }

    public async Task<Result> RemoveGeneralChatSubject(long subjectId, CancellationToken ct = default)
    {
        const string sql = """
                           DELETE FROM general_chat_subjects
                           WHERE general_chat_subject_id = @subjectId
                           """;
        
        var parameters = new { subjectId };
        CommandDefinition command = new(sql, parameters, cancellationToken: ct);
        using var connection = _connectionFactory.Create();
        int rowsAffected = await connection.ExecuteAsync(command);
        return rowsAffected == 0 ? Result.Failure("Не удалось удалить задачу") : Result.Success();
    }
    
    public async Task<Result> RemoveThemeChatSubject(long subjectId, CancellationToken ct = default)
    {
        const string sql = """
                           DELETE FROM theme_chat_subjects WHERE
                           theme_chat_subject_id = @subjectId
                           """;
        
        var parameters = new { subjectId };
        var command = new CommandDefinition(sql, parameters, cancellationToken: ct);
        using var connection = _connectionFactory.Create();
        int rowsAffected = await connection.ExecuteAsync(command);
        return rowsAffected == 0 ? Result.Failure("Не удалось удалить задачу") : Result.Success();
    }

    public void Dispose()
    {
        _context.Dispose();
        context.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
        await context.DisposeAsync();
    }
}
