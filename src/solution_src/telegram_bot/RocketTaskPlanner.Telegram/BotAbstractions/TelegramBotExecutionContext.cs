using CSharpFunctionalExtensions;
using PRTelegramBot.Extensions;
using PRTelegramBot.Interfaces;
using PRTelegramBot.Models;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace RocketTaskPlanner.Telegram.BotAbstractions;

/// <summary>
/// Контекст для выполнения обработчиков telegram команд поэтапно.
/// </summary>
public sealed class TelegramBotExecutionContext
{
    /// <summary>
    /// Входной обработчик, должен вызываться первым
    /// </summary>
    private readonly ITelegramBotHandler? _entryPointHandler;

    /// <summary>
    /// Другие обработчики, добавленные в контекст
    /// </summary>
    private readonly List<ITelegramBotHandler> _handlers = [];

    /// <summary>
    /// Текущий этап
    /// </summary>
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

    /// <summary>
    /// Установка начального обработчика в контексте.
    /// </summary>
    /// <param name="handler">Обработчик</param>
    /// <returns>Новый экземпляр с проинциализированным начальным обработчиком.</returns>
    public TelegramBotExecutionContext SetEntryPointHandler(ITelegramBotHandler handler) =>
        new(this, handler);

    /// <summary>
    /// Вызов начального обработчика
    /// </summary>
    /// <param name="botClient">Telegram bot клиент для общения с telegram</param>
    /// <param name="update">Последнее событие</param>
    /// <exception cref="NullReferenceException">Исключение, если не был проинициализирован начальный обработчик</exception>
    public async Task InvokeEntryPoint(ITelegramBotClient botClient, Update update)
    {
        if (_entryPointHandler == null)
            throw new NullReferenceException(
                "Cannot invoke execution context. Entry point was not set."
            );
        await _entryPointHandler.Handle(botClient, update);
    }

    /// <summary>
    /// Назначение следующего обработчика. С кешем
    /// </summary>
    /// <param name="update">Последнее событие</param>
    /// <param name="handler">Обработчик</param>
    /// <param name="cache">Кеш</param>
    public void AssignNextStep(Update update, ITelegramBotHandler handler, ITelegramCache cache)
    {
        _currentStep = new StepTelegram(handler.Handle, cache);
        update.RegisterStepHandler(_currentStep);
    }

    /// <summary>
    /// Назначение следующего обработчика. Без кеша
    /// </summary>
    /// <param name="update">Последнее событие</param>
    /// <param name="handler">Обработчик</param>
    public void AssignNextStep(Update update, ITelegramBotHandler handler)
    {
        _currentStep = new StepTelegram(handler.Handle);
        update.RegisterStepHandler(_currentStep);
    }

    /// <summary>
    /// Назначение следующего обработчика и его немедленный запуск. С кешем
    /// </summary>
    /// <param name="client">Telegram bot клиент для общения с telegram</param>
    /// <param name="update">Последнее событие</param>
    /// <param name="handler">Обработчик</param>
    /// <param name="cache">Кеш</param>
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

    /// <summary>
    /// Назначение следующего обработчика и его немедленный запуск. Без кеша
    /// </summary>
    /// <param name="client">Telegram bot клиент для общения с telegram</param>
    /// <param name="update">Последнее событие</param>
    /// <param name="handler">Обработчик</param>
    public async Task AssignAndRun(
        ITelegramBotClient client,
        Update update,
        ITelegramBotHandler handler
    )
    {
        AssignNextStep(update, handler);
        await handler.Handle(client, update);
    }

    /// <summary>
    /// Добавление обработчика в контекст
    /// </summary>
    /// <param name="handler">Обработчик</param>
    /// <returns>Контекст с новым списком обработчиков.</returns>
    public TelegramBotExecutionContext RegisterHandler(ITelegramBotHandler handler)
    {
        _handlers.Add(handler);
        return new TelegramBotExecutionContext(this, _handlers);
    }

    /// <summary>
    /// Получение обработчика по его Command свойству.
    /// </summary>
    /// <param name="command">название command</param>
    /// <returns>Обработчик</returns>
    /// <exception cref="NullReferenceException">Исключение, если обработчик с command не найден</exception>
    public ITelegramBotHandler GetRequiredHandler(string command)
    {
        ITelegramBotHandler handler =
            _handlers.FirstOrDefault(h => h.Command == command)
            ?? throw new NullReferenceException("Handler was not registered in execution context.");

        return handler;
    }

    /// <summary>
    /// Получение информации о кеше
    /// </summary>
    /// <typeparam name="T">Кеш который был помещён в контекст</typeparam>
    /// <returns>Кеш. Success если кеш существует, Failure если кеша нет</returns>
    public Result<T> GetCacheInfo<T>()
    {
        if (_currentStep == null)
            return Result.Failure<T>("Cache info not found");

        T cache = _currentStep.GetCache<T>();
        return cache;
    }

    /// <summary>
    /// Очистка всех обработчиков как в контексте, так и в telegram
    /// </summary>
    /// <param name="update">Последнее событие</param>
    public void ClearHandlers(Update update)
    {
        _handlers.Clear();

        if (update.HasStepHandler())
            update.ClearStepUserHandler();
    }

    /// <summary>
    /// Очистка кеша
    /// </summary>
    /// <param name="update">Последнее событие</param>
    public void ClearCacheData(Update update)
    {
        if (update.HasCacheData())
            update.ClearCacheData();
    }
}
