namespace RocketTaskPlanner.Telegram.BotAbstractions;

/// <summary>
/// Dto для содержания информации о пользователе Telegram
/// </summary>
/// <param name="Id">Id пользователя</param>
/// <param name="FirstName">Первое имя</param>
/// <param name="LastName">Второе имя</param>
public sealed record TelegramBotUser(long Id, string FirstName, string? LastName)
{
    /// <summary>
    /// Создание одной строки из двух строк FirstName и LastName
    /// </summary>
    /// <returns>Строка с информацией о имене пользователя</returns>
    public string CombineNamesAsNickname() =>
        LastName switch
        {
            null => FirstName,
            not null => $"{FirstName} {LastName}",
        };
}
