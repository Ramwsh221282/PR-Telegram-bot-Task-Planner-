namespace RocketTaskPlanner.Telegram.BotAbstractions;

public sealed record TelegramBotUser(long Id, string FirstName, string? LastName)
{
    public string CombineNamesAsNickname()
    {
        return LastName switch
        {
            null => FirstName,
            not null => $"{FirstName} {LastName}",
        };
    }
}
