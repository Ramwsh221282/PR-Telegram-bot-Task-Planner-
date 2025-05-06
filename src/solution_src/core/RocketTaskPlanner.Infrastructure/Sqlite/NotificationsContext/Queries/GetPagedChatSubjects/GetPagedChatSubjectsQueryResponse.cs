using System.Diagnostics;

namespace RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.Queries.GetPagedChatSubjects;

public sealed record GetPagedChatSubjectsQueryResponse(SubjectsResponse[] Subjects);

public abstract record SubjectsResponse;

public sealed record GeneralChatSubjectsQueryResponse(
    long ChatId,
    long SubjectId,
    string SubjectText
) : SubjectsResponse;

public sealed record ThemeChatSubjectsQueryResponse(
    long ChatId,
    long ThemeId,
    long SubjectId,
    string SubjectText
) : SubjectsResponse;

public static class GetSubjectsQueryResponseExtensions
{
    public static long GetSubjectChatId(this SubjectsResponse response) =>
        response switch
        {
            GeneralChatSubjectsQueryResponse general => general.ChatId,
            _ => throw new UnreachableException(),
        };

    public static long GetSubjectThemeId(this SubjectsResponse response) =>
        response switch
        {
            ThemeChatSubjectsQueryResponse theme => theme.ThemeId,
            _ => throw new UnreachableException(),
        };

    public static long GetSubjectId(this SubjectsResponse response) =>
        response switch
        {
            GeneralChatSubjectsQueryResponse general => general.SubjectId,
            ThemeChatSubjectsQueryResponse theme => theme.SubjectId,
            _ => throw new UnreachableException(),
        };

    public static string GetSubjectMessage(this SubjectsResponse response) =>
        response switch
        {
            GeneralChatSubjectsQueryResponse general => general.SubjectText,
            ThemeChatSubjectsQueryResponse theme => theme.SubjectText,
            _ => throw new UnreachableException(),
        };

    public static bool IsThemeSubject(this SubjectsResponse response) =>
        response switch
        {
            GeneralChatSubjectsQueryResponse => false,
            ThemeChatSubjectsQueryResponse => true,
            _ => throw new UnreachableException(),
        };
}
