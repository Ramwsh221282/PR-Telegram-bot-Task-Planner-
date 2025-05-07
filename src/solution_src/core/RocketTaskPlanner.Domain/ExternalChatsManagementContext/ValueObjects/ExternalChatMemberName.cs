namespace RocketTaskPlanner.Domain.ExternalChatsManagementContext.ValueObjects;

/// <summary>
/// Никнейм обладателя чата
/// </summary>
public sealed record ExternalChatMemberName
{
    /// <summary>
    /// Никнейм
    /// </summary>
    public string Value { get; }

    private ExternalChatMemberName() => Value = string.Empty;

    private ExternalChatMemberName(string value) => Value = value;

    public static Result<ExternalChatMemberName> Create(string? name)
    {
        return string.IsNullOrWhiteSpace(name)
            ? Result.Failure<ExternalChatMemberName>("Имя участника чата было пустым")
            : new ExternalChatMemberName(name);
    }
}
