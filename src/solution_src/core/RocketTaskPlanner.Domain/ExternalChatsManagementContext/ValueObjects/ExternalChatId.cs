namespace RocketTaskPlanner.Domain.ExternalChatsManagementContext.ValueObjects;

/// <summary>
/// ID внешнего чата пользователя
/// </summary>
public readonly record struct ExternalChatId
{
    /// <summary>
    /// ID
    /// </summary>
    public long Value { get; }

    public ExternalChatId() => Value = -1;

    public ExternalChatId(long value) => Value = value;

    public static ExternalChatId Dedicated(long id) => new(id);
}
