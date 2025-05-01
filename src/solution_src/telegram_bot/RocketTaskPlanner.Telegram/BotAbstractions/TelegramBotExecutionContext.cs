using CSharpFunctionalExtensions;
using PRTelegramBot.Extensions;
using PRTelegramBot.Interfaces;
using PRTelegramBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotAbstractions;

public sealed class TelegramBotExecutionContext
{
    private readonly ITelegramBotHandler? _entryPointHandler;
    private readonly List<ITelegramBotHandler> _handlers = [];
    private StepTelegram? _currentStep;

    public TelegramBotExecutionContext() { }

    private TelegramBotExecutionContext(
        TelegramBotExecutionContext context,
        ITelegramBotHandler entryPointHandler
    )
    {
        _handlers = context._handlers;
        _currentStep = context._currentStep;
        _entryPointHandler = entryPointHandler;
    }

    private TelegramBotExecutionContext(
        TelegramBotExecutionContext context,
        List<ITelegramBotHandler> handlers
    )
    {
        _handlers = handlers;
        _currentStep = context._currentStep;
        _entryPointHandler = context._entryPointHandler;
    }

    public TelegramBotExecutionContext SetEntryPointHandler(ITelegramBotHandler handler) =>
        new(this, handler);

    public async Task InvokeEntryPoint(ITelegramBotClient botClient, Update update)
    {
        if (_entryPointHandler == null)
            throw new NullReferenceException(
                "Cannot invoke execution context. Entry point was not set."
            );
        await _entryPointHandler.Handle(botClient, update);
    }

    public void AssignNextStep(Update update, ITelegramBotHandler handler, ITelegramCache cache)
    {
        _currentStep = new StepTelegram(handler.Handle, cache);
        update.RegisterStepHandler(_currentStep);
    }

    public void AssignNextStep(Update update, ITelegramBotHandler handler)
    {
        _currentStep = new StepTelegram(handler.Handle);
        update.RegisterStepHandler(_currentStep);
    }

    public async Task AssignAndRun(
        ITelegramBotClient client,
        Update update,
        ITelegramBotHandler handler,
        ITelegramCache cache
    )
    {
        AssignNextStep(update, handler, cache);
        await handler.Handle(client, update);
    }

    public async Task AssignAndRun(
        ITelegramBotClient client,
        Update update,
        ITelegramBotHandler handler
    )
    {
        AssignNextStep(update, handler);
        await handler.Handle(client, update);
    }

    public TelegramBotExecutionContext RegisterHandler(ITelegramBotHandler handler)
    {
        _handlers.Add(handler);
        return new TelegramBotExecutionContext(this, _handlers);
    }

    public ITelegramBotHandler GetRequiredHandler(string command)
    {
        ITelegramBotHandler? handler =
            _handlers.FirstOrDefault(h => h.Command == command)
            ?? throw new NullReferenceException("Handler was not registered in execution context.");

        return handler;
    }

    public Result<T> GetCacheInfo<T>()
    {
        if (_currentStep == null)
            return Result.Failure<T>("Cache info not found");

        T cache = _currentStep.GetCache<T>();
        return cache;
    }

    public void ClearHandlers(Update update)
    {
        _handlers.Clear();

        if (update.HasStepHandler())
            update.ClearStepUserHandler();
    }

    public void ClearCacheData(Update update)
    {
        if (update.HasCacheData())
            update.ClearCacheData();
    }
}
