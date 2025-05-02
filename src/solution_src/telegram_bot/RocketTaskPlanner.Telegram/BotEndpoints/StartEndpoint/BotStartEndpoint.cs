using PRTelegramBot.Attributes;
using PRTelegramBot.Models.Enums;
using RocketTaskPlanner.Application.ApplicationTimeContext.Features.SaveTimeZoneDbApiKey;
using RocketTaskPlanner.Application.PermissionsContext.Repository;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Application.UsersContext.Contracts;
using RocketTaskPlanner.Application.UsersContext.Features.AddUser;
using RocketTaskPlanner.Application.UsersContext.Features.AddUserPermission;
using RocketTaskPlanner.Domain.UsersContext.Entities;
using RocketTaskPlanner.Infrastructure.Abstractions;
using RocketTaskPlanner.Infrastructure.Sqlite.ApplicationTimeContext.Queries.HasTimeZoneDbToken;
using RocketTaskPlanner.Infrastructure.TimeZoneDb;
using RocketTaskPlanner.Telegram.BotAbstractions;
using RocketTaskPlanner.Telegram.BotEndpoints.StartEndpoint.Handlers.OnContinue;
using RocketTaskPlanner.Telegram.BotEndpoints.StartEndpoint.Handlers.OnStart;
using RocketTaskPlanner.Telegram.BotEndpoints.StartEndpoint.Handlers.OnTokenReply;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotEndpoints.StartEndpoint;

/// <summary>
/// Endpoint обработок для команды /start
/// Здесь происходит конфигурирование Time Zone Db ключа.
/// </summary>
[BotHandler]
public sealed class BotStartEndpoint
{
    /// <summary>
    /// Контекст исполнения обработок при вызове команды /start.
    /// Здесь содержаться обработчики шагов этапа создания Time Zone Db ключа.
    /// </summary>
    private readonly TelegramBotExecutionContext _context;

    /// <summary>
    /// Конструктор класса TimeZoneDbApiKeySetupEndpoint.
    /// </summary>
    /// <param name="addOwnerUseCase">Обработчик для создания обладателя бота.</param>
    /// <param name="addOwnerPermissionsUseCase">Обработчик для добавление обладателю прав.</param>
    /// <param name="permissionsRepository">Репозиторий прав.</param>
    /// <param name="usersRepository">Репозиторий пользователей.</param>
    /// <param name="saveTimeZoneKeyUseCaseHandler">Обработчик сохранения Time Zone Db ключа.</param>
    /// <param name="hasTimeZoneQueryHandler">Обработчик проверки на существование Time Zone Db ключа.</param>
    public BotStartEndpoint(
        IUseCaseHandler<
            SaveTimeZoneDbApiKeyUseCase,
            TimeZoneDbProvider
        > saveTimeZoneKeyUseCaseHandler,
        IQueryHandler<
            HasTimeZoneDbTokenQuery,
            HasTimeZoneDbTokenQueryResponse
        > hasTimeZoneQueryHandler,
        IUseCaseHandler<AddUserPermissionUseCase, UserPermission> addOwnerPermissionsUseCase,
        IUseCaseHandler<AddUserUseCase, RocketTaskPlanner.Domain.UsersContext.User> addOwnerUseCase,
        IPermissionsRepository permissionsRepository,
        IUsersRepository usersRepository
    )
    {
        _context = new TelegramBotExecutionContext();
        OnTimeZoneDbTokenReplyHandler replyHandler = new(
            _context,
            saveTimeZoneKeyUseCaseHandler,
            addOwnerUseCase,
            addOwnerPermissionsUseCase,
            permissionsRepository
        );
        OnTimeZoneDbTokenContinueHandler continueHandler = new(_context);
        OnTimeZoneDbTokenKeyStartHandler entryPoint = new(
            _context,
            hasTimeZoneQueryHandler,
            usersRepository
        );

        _context = _context
            .SetEntryPointHandler(entryPoint)
            .RegisterHandler(continueHandler)
            .RegisterHandler(replyHandler);
    }

    /// <summary>
    /// Точка входа в команду /start
    /// </summary>
    /// <param name="client">Сконфигурированный телеграм-бот клиент.</param>
    /// <param name="update">Событие о пользовательском вводе.</param>
    [ReplyMenuHandler(CommandComparison.Contains, StringComparison.OrdinalIgnoreCase, ["/start"])]
    public async Task OnStart(ITelegramBotClient client, Update update) =>
        await _context.InvokeEntryPoint(client, update);
}
