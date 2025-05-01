namespace RocketTaskPlanner.Domain.UsersContext.ValueObjects;

public sealed record UserName
{
    public string Value { get; }

    private UserName() => Value = string.Empty; // ef core

    private UserName(string value) => Value = value;

    public static Result<UserName> Create(string userName) =>
        string.IsNullOrWhiteSpace(userName)
            ? Result.Failure<UserName>("Имя пользователя было пустым.")
            : new UserName(userName);
}
