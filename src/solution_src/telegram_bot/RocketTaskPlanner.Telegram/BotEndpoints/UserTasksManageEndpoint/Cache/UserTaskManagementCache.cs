using PRTelegramBot.Interfaces;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.GetNotificationReceiversByIdArray;
using RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.GetPagedChatSubjects;

namespace RocketTaskPlanner.Telegram.BotEndpoints.UserTasksManageEndpoint.Cache;

public sealed record UserTaskManagementCache : ITelegramCache
{
    private GetNotificationReceiversByIdentifiersQueryResponse? _selectedChat;

    private SubjectsResponse? _selectedSubject;
    public GetNotificationReceiversByIdentifiersQueryResponse[] UserChats { get; }
    public SubjectsResponse[] CurrentTasks { get; init; } = [];
    public int CurrentPage { get; init; } = 1;
    public bool IsPeriodicView { get; init; }
    public int PageSize { get; } = 10;
    public int PagesCount { get; init; } = -1;

    public UserTaskManagementCache(
        GetNotificationReceiversByIdentifiersQueryResponse[] userChats
    ) => UserChats = userChats;

    public bool ClearData() => true;

    public GetNotificationReceiversByIdentifiersQueryResponse GetChat(string chatName)
    {
        var requiredChat = UserChats.FirstOrDefault(c => c.ChatName == chatName);
        return requiredChat
            ?? throw new ArgumentNullException(nameof(requiredChat), "required chat was not found");
    }

    public UserTaskManagementCache SetSelectedChat(
        GetNotificationReceiversByIdentifiersQueryResponse chat
    )
    {
        _selectedChat = chat;
        return this;
    }

    public UserTaskManagementCache SetSelectedSubject(SubjectsResponse subject)
    {
        _selectedSubject = subject;
        return this;
    }

    public UserTaskManagementCache ClearSelectedSubject()
    {
        _selectedSubject = null;
        return this;
    }

    public UserTaskManagementCache ClearSelectedChat()
    {
        _selectedChat = null;
        return this;
    }

    public SubjectsResponse GetSelectedSubject() =>
        _selectedSubject
        ?? throw new ArgumentNullException(
            nameof(_selectedSubject),
            "selected subject has not been set"
        );

    public SubjectsResponse GetSubjectById(long subjectId)
    {
        var subject = CurrentTasks.FirstOrDefault(c => c.GetSubjectId() == subjectId);
        return subject
            ?? throw new ArgumentNullException(nameof(subjectId), "required subjet was not found");
    }

    public long GetSelectedChatId() =>
        _selectedChat?.ChatId ?? throw new ArgumentNullException(nameof(_selectedChat));
}
