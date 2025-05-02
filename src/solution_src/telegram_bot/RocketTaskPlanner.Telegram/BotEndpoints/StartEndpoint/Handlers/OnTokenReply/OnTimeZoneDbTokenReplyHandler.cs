using CSharpFunctionalExtensions;
using PRTelegramBot.Extensions;
using PRTelegramBot.Models;
using RocketTaskPlanner.Application.ApplicationTimeContext.Features.SaveTimeZoneDbApiKey;
using RocketTaskPlanner.Application.PermissionsContext.Repository;
using RocketTaskPlanner.Application.Shared.UseCaseHandler;
using RocketTaskPlanner.Application.UsersContext.Features.AddUser;
using RocketTaskPlanner.Application.UsersContext.Features.AddUserPermission;
using RocketTaskPlanner.Domain.PermissionsContext;
using RocketTaskPlanner.Domain.UsersContext.Entities;
using RocketTaskPlanner.Infrastructure.TimeZoneDb;
using RocketTaskPlanner.Telegram.BotAbstractions;
using RocketTaskPlanner.Telegram.BotConstants;
using RocketTaskPlanner.Telegram.BotExtensions;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotEndpoints.StartEndpoint.Handlers.OnTokenReply;

public sealed class OnTimeZoneDbTokenReplyHandler(
    TelegramBotExecutionContext context,
    IUseCaseHandler<SaveTimeZoneDbApiKeyUseCase, TimeZoneDbProvider> useCaseHandler,
    IUseCaseHandler<AddUserUseCase, Domain.UsersContext.User> addUserHandler,
    IUseCaseHandler<AddUserPermissionUseCase, UserPermission> addPermisionHandler,
    IPermissionsRepository permissionsRepository
) : ITelegramBotHandler
{
    private readonly TelegramBotExecutionContext _context = context;

    private readonly IUseCaseHandler<
        SaveTimeZoneDbApiKeyUseCase,
        TimeZoneDbProvider
    > _useCaseHandler = useCaseHandler;

    private readonly IUseCaseHandler<AddUserUseCase, Domain.UsersContext.User> _addUserHandler =
        addUserHandler;

    private readonly IUseCaseHandler<
        AddUserPermissionUseCase,
        UserPermission
    > _addPermisionHandler = addPermisionHandler;

    private readonly IPermissionsRepository _permissionsRepository = permissionsRepository;

    public string Command => TimeZoneDbApiKeyManagementConstants.TokenReplyCommand;

    public async Task Handle(ITelegramBotClient client, Update update)
    {
        Result<string> message = update.GetMessage();
        if (message.IsFailure)
        {
            await message.SendError(client, update);
            return;
        }

        await update.RemoveLastMessage(client);
        StepTelegram? previous = update.GetStepHandler<StepTelegram>();
        if (previous == null)
            return;

        Result<TelegramBotUser> userResult = update.GetUser();
        if (userResult.IsFailure)
        {
            await userResult.SendError(client, update);
            return;
        }
        TelegramBotUser user = userResult.Value;

        Result addingUser = await AddFirstUser(user);
        if (addingUser.IsFailure)
        {
            await addingUser.SendError(client, update);
            return;
        }

        Result addingPermissions = await AddFirstUserOwnerPermissions(user);
        if (addingPermissions.IsFailure)
        {
            await addingPermissions.SendError(client, update);
            return;
        }

        Result savingTimeZoneApiKey = await SaveTimeZoneDbApiKey(message.Value);
        if (savingTimeZoneApiKey.IsFailure)
        {
            await savingTimeZoneApiKey.SendError(client, update);
            return;
        }

        await client.RegisterBotOwnerCommands(update.GetChatId());
        _context.ClearHandlers(update);
        _context.ClearCacheData(update);
        OptionMessage replyMessageOption = new() { ClearMenu = true };
        await PRTelegramBot.Helpers.Message.Send(
            client,
            update,
            TimeZoneDbApiKeyManagementConstants.ReplyMessageOnSuccess,
            replyMessageOption
        );
    }

    private async Task<Result> SaveTimeZoneDbApiKey(string message)
    {
        SaveTimeZoneDbApiKeyUseCase useCase = new(message);
        Result<TimeZoneDbProvider> provider = await _useCaseHandler.Handle(useCase);
        return provider;
    }

    private async Task<Result> AddFirstUser(TelegramBotUser user)
    {
        long id = user.Id;
        string name = user.CombineNamesAsNickname();
        AddUserUseCase useCase = new(id, name);
        Result<Domain.UsersContext.User> useCaseResult = await _addUserHandler.Handle(useCase);
        return useCaseResult;
    }

    private async Task<Result> AddFirstUserOwnerPermissions(TelegramBotUser user)
    {
        Result<Permission> permissionEditorResult =
            await _permissionsRepository.ReadableRepository.GetByName(
                PermissionNames.EditConfiguration
            );
        if (permissionEditorResult.IsFailure)
            return permissionEditorResult;

        Result<Permission> permissionCreateTasksResult =
            await _permissionsRepository.ReadableRepository.GetByName(PermissionNames.CreateTasks);
        if (permissionCreateTasksResult.IsFailure)
            return permissionCreateTasksResult;

        long userId = user.Id;
        Permission permissionEditor = permissionEditorResult.Value;
        Permission permissionCreateTasks = permissionCreateTasksResult.Value;

        AddUserPermissionUseCase useCaseEditor = new(
            userId,
            permissionEditor.Id,
            permissionEditor.Name
        );
        AddUserPermissionUseCase addUseCaseCreateTasks = new(
            userId,
            permissionCreateTasks.Id,
            permissionCreateTasks.Name
        );

        Result<UserPermission> editorResult = await _addPermisionHandler.Handle(useCaseEditor);
        if (editorResult.IsFailure)
            return editorResult;

        Result<UserPermission> createTasksResult = await _addPermisionHandler.Handle(
            addUseCaseCreateTasks
        );
        if (createTasksResult.IsFailure)
            return createTasksResult;

        return Result.Success();
    }
}
