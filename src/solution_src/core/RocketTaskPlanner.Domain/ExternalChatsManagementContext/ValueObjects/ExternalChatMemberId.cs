namespace RocketTaskPlanner.Domain.ExternalChatsManagementContext.ValueObjects;

public readonly record struct ExternalChatMemberId
{
    public long Value { get; }

    public ExternalChatMemberId() => Value = -1;

    public ExternalChatMemberId(long value) => Value = value;

    public static ExternalChatMemberId Dedicated(long id) => new(id);
}
