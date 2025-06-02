using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using RocketTaskPlanner.Application.ExternalChatsManagementContext.Repository;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext.ValueObjects;
using RocketTaskPlanner.Infrastructure.Abstractions;

namespace RocketTaskPlanner.Infrastructure.Database.ExternalChatsManagementContext.Repositories;

/// <summary>
/// Абстракция для взаимодействия с БД пользователей и пользовательских чатов (операции записи)
/// </summary>
public sealed class ExternalChatsWritableRepository : IExternalChatsWritableRepository, IAsyncDisposable
{
    /// <summary>
    /// <inheritdoc cref="IDbConnectionFactory"/>
    /// </summary>
    private readonly RocketTaskPlannerDbContext _context;

    public ExternalChatsWritableRepository(RocketTaskPlannerDbContext context) =>
        _context = context;

    /// <summary>
    /// Добавить обладателя чата
    /// <param name="externalChatOwner">
    ///     <inheritdoc cref="ExternalChatOwner"/>
    /// </param>
    /// <param name="ct">Токен отмены</param>
    /// <returns>Result Success</returns>
    /// </summary>
    public async Task<Result<ExternalChatOwner>> AddChatOwner(ExternalChatOwner externalChatOwner, CancellationToken ct = default)
    {
        if (await _context.Owners.AsNoTracking().AnyAsync(o => o.Id == externalChatOwner.Id, cancellationToken: ct))
            return Result.Failure<ExternalChatOwner>($"Уже есть пользователь-обладатель чатов с ID: {externalChatOwner.Id.Value}");

        await _context.Owners.AddAsync(externalChatOwner, ct);
        return externalChatOwner;
    }

    public void RemoveChatOwner(ExternalChatOwner externalChatOwner) =>
        _context.Owners.Remove(externalChatOwner);

    public async Task<Result<ExternalChatOwner>> GetById(long ownerId, CancellationToken ct = default)
    {
        var owner = await _context.Owners
            .Include(o => o.Chats)
            .FirstOrDefaultAsync(o => o.Id == ExternalChatMemberId.Dedicated(ownerId), cancellationToken: ct);

        return owner ?? Result.Failure<ExternalChatOwner>($"Не найден обладатель чатов с ID: {ownerId}");
    }

    public void Dispose()
    {
        _context.Dispose();
    }

    public async ValueTask DisposeAsync()
    {
        await _context.DisposeAsync();
    }
}
