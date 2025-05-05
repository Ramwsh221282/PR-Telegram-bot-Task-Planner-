namespace RocketTaskPlanner.Domain.ExternalChatsManagementContext.ValueObjects;

public readonly record struct ExternalChatId
{
    public long Value { get; }

    public ExternalChatId() => Value = -1;

    public ExternalChatId(long value) => Value = value;

    public static ExternalChatId Dedicated(long id) => new(id);
}
