namespace RocketTaskPlanner.Domain.UsersContext.ValueObjects;

public readonly record struct UserId
{
    public long Value { get; }

    public UserId()
    {
        Value = 0;
    }

    private UserId(long value)
    {
        Value = value;
    }

    public static UserId Create(long value) => new(value);
}
