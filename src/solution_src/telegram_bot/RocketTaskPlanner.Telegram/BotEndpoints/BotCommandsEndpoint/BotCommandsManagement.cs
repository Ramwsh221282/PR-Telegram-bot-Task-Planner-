using CSharpFunctionalExtensions;
using PRTelegramBot.Attributes;
using PRTelegramBot.Extensions;
using RocketTaskPlanner.Application.UsersContext.Contracts;
using RocketTaskPlanner.Domain.PermissionsContext;
using RocketTaskPlanner.Domain.UsersContext.ValueObjects;
using RocketTaskPlanner.Telegram.BotExtensions;
using Telegram.Bot;
using Telegram.Bot.Types;
using User = RocketTaskPlanner.Domain.UsersContext.User;

namespace RocketTaskPlanner.Telegram.BotEndpoints.BotCommandsEndpoint;

[BotHandler]
public class BotCommandsManagement
{
    private readonly IUsersReadableRepository _repository;

    public BotCommandsManagement(IUsersReadableRepository repository)
    {
        _repository = repository;
    }

    [ReplyMenuHandler("/clear_commands")]
    public async Task ClearCommandsHandler(ITelegramBotClient client, Update update)
    {
        Result<User> ownerRequest = await GetOwner(update);
        if (ownerRequest.IsFailure)
        {
            await ownerRequest.SendError(client, update);
            return;
        }

        long chatId = update.GetChatId();
        BotCommandScopeChat scope = BotCommandScope.Chat(chatId);
        await client.DeleteMyCommands(scope);
        await client.SetMyCommands(
            [new BotCommand("/create_default_commands", "Возврат меню команд")]
        );
        await PRTelegramBot.Helpers.Message.Send(client, update, "Меню команд было очищено.");
    }

    [ReplyMenuHandler("/create_default_commands")]
    public async Task RecreateCommandsHandler(ITelegramBotClient client, Update update)
    {
        Result<User> ownerRequest = await GetOwner(update);
        if (ownerRequest.IsFailure)
        {
            await ownerRequest.SendError(client, update);
            return;
        }

        long chatId = update.GetChatId();
        BotCommandScopeChat scope = BotCommandScope.Chat(chatId);
        List<BotCommand> commands =
        [
            new("/time_zone_token_configure", "Настройка Time Zone Db ключа."),
            new("/add_this_chat", "Зарегистрировать чат в боте. Используется не в чате с ботом."),
            new("/remove_this_chat", "Удалить чат из бота. Используется не в чате с ботом."),
            new("/tc", "Создать задачу - /tc <Текст задачи>. Используется не в чате с ботом."),
            new("/chat_time", "Узнать время чата. Используется не в чате с ботом."),
            new("/chat_time_reconfigure", "Сброс времени чата. Используется не в чате с ботом."),
            new("/clear_commands", "Очистить команды в меню"),
            new("/info", "Справка"),
        ];
        await client.SetMyCommands(commands, scope);
        await PRTelegramBot.Helpers.Message.Send(client, update, "Меню команд было создано.");
    }

    private async Task<Result<User>> GetOwner(Update update)
    {
        Result<long> userIdResult = update.GetUserId();
        if (userIdResult.IsFailure)
            return Result.Failure<User>(userIdResult.Error);

        Result<User> user = await _repository.GetById(UserId.Create(userIdResult.Value));
        if (user.IsFailure)
            return Result.Failure<User>(user.Error);

        string editorPermission = PermissionNames.EditConfiguration;
        bool hasPermission = user.Value.HasPermission(editorPermission);
        return !hasPermission
            ? Result.Failure<User>("Операция доступна только обладателю.")
            : user.Value;
    }
}
