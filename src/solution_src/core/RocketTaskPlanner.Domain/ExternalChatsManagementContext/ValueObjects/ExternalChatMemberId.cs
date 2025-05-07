namespace RocketTaskPlanner.Domain.ExternalChatsManagementContext.ValueObjects;

/// <summary>
/// ID обладателя чата
/// </summary>
public readonly record struct ExternalChatMemberId
{
    /// <summary>
    /// ID
    /// </summary>
    public long Value { get; }

    public ExternalChatMemberId() => Value = -1;

    public ExternalChatMemberId(long value) => Value = value;

    public static ExternalChatMemberId Dedicated(long id) => new(id);
}
