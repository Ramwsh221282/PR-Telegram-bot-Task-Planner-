namespace RocketTaskPlanner.Telegram.BotEndpoints.StartEndpoint.Handlers;

public class TimeZoneDbApiKeyManagementConstants
{
    public const string ReplyMessageOnStart = """
        Вас приветствует телеграм-бот планировщик задач 👋.

        Для корректной работы бота необходимо выполнить первоначальную настройку ⚙️.

        Бот взаимодействует с сервисом Time Zone Db 📡.

        Зарегистрируйтесь на https://timezonedb.com чтобы получить ключ, который понадобится для настройки бота. ✏️

        Чтобы остановить настройку, Вы можете написать команду: /stop_config 🛑.
        Либо воспользуйтесь кнопкой Отменить в меню 🛑.

        Когда Вы зарегистрируетесь нажмите Продолжить ➡️
        """;

    public const string ReplyMessageOnUpdateKey = """
        Вы попали в опцию изменения токена Time Zone Db 👋.

        Для изменения токена, Вы должны быть зарегистрированы на платформе:

        https://timezonedb.com

        Если вы зарегистрированы на платформе, нажмите Продолжить ➡️.
        """;

    public const string ReplyMessageOnContinue = """
        Хорошо, теперь, отправьте мне API токен (API Key) 🙏.

        Найти API токен от Time Zone Db можно по следующей ссылке 👉:

        https://timezonedb.com/account
        """;

    public const string ReplyMessageOnSuccess = """
        Отлично. Я сохранил токен Time Zone Db в своей конфигурации ✅.
        Данный токен является бессрочным ♾️.

        Теперь, после того, как я знаю ключ от Time Zone Db сервиса,
        Вы можете установить мне время, в котором я буду работать 🕔.

        Для этого вызовите команду /time_config

        Вы можете в любое время поменять токен Time Zone Db ℹ️.
        Для этого вызовите команду /update_time_api_key
        """;

    public const string UpdateCommand = "/update_time_api_key";
    public const string StartCommand = "/start";
    public const string ContinueCommand = "/continue_tz_token_configuration";
    public const string CancelCommand = "/stop_config";
    public const string TokenReplyCommand = "/tz_token_reply";
}
