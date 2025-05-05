using RocketTaskPlanner.Domain.ExternalChatsManagementContext.ValueObjects;

namespace RocketTaskPlanner.Domain.ExternalChatsManagementContext.Entities;

/// <summary>
/// Внешний чат.
/// </summary>
public sealed class ExternalChat
{
    /// <summary>
    /// ID чата
    /// </summary>
    public ExternalChatId Id { get; }

    /// <summary>
    /// ID родителя (если это чат темы).
    /// </summary>
    public ExternalChatId? ParentId { get; }

    /// <summary>
    /// Название внешнего чата
    /// </summary>
    public ExternalChatName Name { get; } = null!;

    /// <summary>
    /// Обладатель внешнего чата
    /// </summary>
    public ExternalChatOwner Owner { get; } = null!;

    /// <summary>
    /// ID ообладателя внешнего чата
    /// </summary>
    public ExternalChatMemberId OwnerId { get; }

    private ExternalChat() { } // ef core

    /// <summary>
    /// Создание внешнего основного чата (вызывается в ExternalChatOwner)
    /// </summary>
    /// <param name="owner">Обладатель чата</param>
    /// <param name="chatId">Id чата</param>
    /// <param name="chatName">Название чата</param>
    internal ExternalChat(ExternalChatOwner owner, ExternalChatId chatId, ExternalChatName chatName)
    {
        Owner = owner;
        OwnerId = owner.Id;
        Id = chatId;
        Name = chatName;
    }

    public ExternalChat(
        ExternalChatMemberId ownerId,
        ExternalChatId chatId,
        ExternalChatName chatName
    )
    {
        OwnerId = ownerId;
        Id = chatId;
        Name = chatName;
    }

    internal ExternalChat(
        ExternalChatOwner owner,
        ExternalChat parent,
        ExternalChatId themeId,
        ExternalChatName themeName
    )
    {
        Owner = owner;
        OwnerId = owner.Id;
        Name = themeName;
        Id = themeId;
        ParentId = parent.Id;
    }
}
