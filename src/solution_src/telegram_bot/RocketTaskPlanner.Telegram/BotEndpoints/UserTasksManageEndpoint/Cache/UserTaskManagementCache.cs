using System.Diagnostics;
using PRTelegramBot.Interfaces;
using RocketTaskPlanner.Infrastructure.Database.NotificationsContext.Queries.GetNotificationReceiversByIdArray;
using RocketTaskPlanner.Infrastructure.Database.NotificationsContext.Queries.GetPagedChatSubjects;

namespace RocketTaskPlanner.Telegram.BotEndpoints.UserTasksManageEndpoint.Cache;

public abstract record UserTaskManagementChat
{
    public abstract string MetadataInformation();

    public abstract UserTaskManagementChat? Specify(string chatName);
}

public sealed record UserTaskManagementGeneralChat(string Name, long Id) : UserTaskManagementChat
{
    private string? _metadata;
    public override string MetadataInformation() => _metadata ??= $"Чат: {Name} ID: {Id}";
    public override UserTaskManagementChat? Specify(string chatName)
    {
        return MetadataInformation() != chatName ? null : this;
    }
}

public sealed record UserTaskManagementThemeChat(UserTaskManagementGeneralChat GeneralChat, long Id) : UserTaskManagementChat
{
    private string? _metadata;
    public override string MetadataInformation()
    {
        return _metadata ??= $"Тема чата: {GeneralChat.Name} ({GeneralChat.Id}). ID: {Id}";
    }

    public override UserTaskManagementChat? Specify(string chatName)
    {
        return MetadataInformation() != chatName ? null : this;
    }
}

/// <summary>
/// Кеш для работы в контексте обработки команды /my_tasks
/// </summary>
public sealed record UserTaskManagementCache : ITelegramCache
{
    /// <summary>
    /// <inheritdoc cref="GetNotificationReceiversByIdentifiersQueryResponse"/>
    /// </summary>
    private UserTaskManagementChat? _selectedChat;

    /// <summary>
    /// <inheritdoc cref="SubjectsResponse"/>
    /// </summary>
    private SubjectsResponse? _selectedSubject;

    /// <summary>
    /// <inheritdoc cref="GetNotificationReceiversByIdentifiersQueryResponse"/>
    /// </summary>
    public UserTaskManagementChat[] UserChats { get; }

    /// <summary>
    /// <inheritdoc cref="SubjectsResponse"/>
    /// </summary>
    public SubjectsResponse[] CurrentTasks { get; init; } = [];

    /// <summary>
    /// Текущая страница, по умолчанию 1
    /// </summary>
    public int CurrentPage { get; init; } = 1;

    /// <summary>
    /// Режим просмотра задач (периодические, либо не периодические)
    /// </summary>
    public bool IsPeriodicView { get; init; }

    /// <summary>
    /// Размер страницы. Фиксированный - 10. Т.е до 10 элементов для каждой страницы.
    /// </summary>
    public int PageSize { get; } = 10;

    /// <summary>
    /// Количество страниц в целом. По умолчанию -1. (т.е если не проинициализировано).
    /// </summary>
    public int PagesCount { get; init; } = -1;

    public UserTaskManagementCache(
        UserTaskManagementChat[] userChats
    ) => UserChats = userChats;

    /// <summary>
    /// Очистка кеша. Заглушка.
    /// </summary>
    public bool ClearData() => true;

    /// <summary>
    /// Получить чат по названию чата.
    /// </summary>
    /// <param name="chatName">Название чата</param>
    /// <returns>Dto с информацией о чате.</returns>
    /// <exception cref="ArgumentNullException">Исключение, если не найден чат. Если чаты не проинициализированы.</exception>
    public UserTaskManagementChat GetChat(string chatName)
    {
        var requiredChat = UserChats.FirstOrDefault(c => c.Specify(chatName) != null);
        return requiredChat
            ?? throw new ArgumentNullException(nameof(requiredChat), "required chat was not found");
    }

    /// <summary>
    /// Установить выбранный чат.
    /// Т.е когда пользователь в меню выбирает чат, этот метод нужно вызвать.
    /// </summary>
    /// <param name="chat">Чат</param>
    /// <returns>Обновленный кеш</returns>
    public UserTaskManagementCache SetSelectedChat(
        UserTaskManagementChat chat
    )
    {
        _selectedChat = chat;
        return this;
    }

    /// <summary>
    /// Установить выбранную задачу.
    /// Т.е когда пользователь выбирает задачу в меню, этот метод нужно вызвтаь.
    /// </summary>
    /// <param name="subject">Задача</param>
    /// <returns>Обновленный кеш</returns>
    public UserTaskManagementCache SetSelectedSubject(SubjectsResponse subject)
    {
        _selectedSubject = subject;
        return this;
    }

    /// <summary>
    /// Очистить выбранную задачу.
    /// Нужно вызвать когда удаляется задача, либо происходит выход из меню задачи.
    /// </summary>
    /// <returns>Обновленный кеш</returns>
    public UserTaskManagementCache ClearSelectedSubject()
    {
        _selectedSubject = null;
        return this;
    }

    /// <summary>
    /// Очистить выбранный чат.
    /// Нужно вызвать, когда происходит переход из меню выбора типа задач.
    /// </summary>
    /// <returns>Обновленный кеш</returns>
    public UserTaskManagementCache ClearSelectedChat()
    {
        _selectedChat = null;
        return this;
    }

    /// <summary>
    ///
    /// </summary>
    /// <returns></returns>
    /// <exception cref="ArgumentNullException"></exception>
    public SubjectsResponse GetSelectedSubject() =>
        _selectedSubject
        ?? throw new ArgumentNullException(
            nameof(_selectedSubject),
            "selected subject has not been set"
        );

    /// <summary>
    /// Получить задачу по ID.
    /// </summary>
    /// <param name="subjectId">ID задачи</param>
    /// <returns>Задача</returns>
    /// <exception cref="ArgumentNullException">Исключение если задача не найдена</exception>
    public SubjectsResponse GetSubjectById(long subjectId)
    {
        var subject = CurrentTasks.FirstOrDefault(c => c.GetSubjectId() == subjectId);
        return subject
            ?? throw new ArgumentNullException(nameof(subjectId), "required subjet was not found");
    }

    /// <summary>
    /// Получить ID выбранного чата.
    /// </summary>
    /// <returns>ID чата</returns>
    /// <exception cref="ArgumentNullException">Исключение если выбранный чат не проиницализирован</exception>
    public long GetSelectedChatId() =>
        _selectedChat switch
        {
            null => throw new ArgumentNullException(nameof(_selectedChat)),
            UserTaskManagementGeneralChat g => g.Id,
            UserTaskManagementThemeChat t => t.Id,
            _ => throw new UnreachableException(),
        };

    public bool IsSelectedChatTheme() =>
        _selectedChat switch
        {
            null => throw new ArgumentNullException(nameof(_selectedChat)),
            UserTaskManagementGeneralChat => false,
            UserTaskManagementThemeChat => true,
            _ => throw new UnreachableException(),
        };

    public long ThemeParentId() =>
        _selectedChat switch
        {
            null => throw new ArgumentNullException(nameof(_selectedChat)),
            UserTaskManagementGeneralChat => throw new ArgumentException(),
            UserTaskManagementThemeChat t => t.GeneralChat.Id,
            _ => throw new UnreachableException(),
        };
}
