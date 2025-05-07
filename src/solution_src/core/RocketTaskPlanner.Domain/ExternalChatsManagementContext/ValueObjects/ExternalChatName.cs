namespace RocketTaskPlanner.Domain.ExternalChatsManagementContext.ValueObjects;

/// <summary>
/// Название внешнего чата
/// </summary>
public sealed record ExternalChatName
{
    /// <summary>
    /// Название внешнего чата
    /// </summary>
    public string Value { get; }

    private ExternalChatName() => Value = string.Empty;

    private ExternalChatName(string value) => Value = value;

    public static Result<ExternalChatName> Create(string? value) =>
        string.IsNullOrWhiteSpace(value)
            ? Result.Failure<ExternalChatName>("Пустое название внешнего чата.")
            : new ExternalChatName(value);
}
