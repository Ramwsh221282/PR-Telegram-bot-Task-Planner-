using System.Diagnostics;

namespace RocketTaskPlanner.Infrastructure.Database.NotificationsContext.Queries.GetPagedChatSubjects;

/// <summary>
/// Результат запроса на <inheritdoc cref="GetPagedChatSubjectsQuery"/>
/// <param name="Subjects"><inheritdoc cref="SubjectsResponse"/></param>
/// </summary>
public sealed record GetPagedChatSubjectsQueryResponse(SubjectsResponse[] Subjects);

/// <summary>
/// Базовый уведомлений класс для ответа <inheritdoc cref="GetPagedChatSubjectsQuery"/>
/// </summary>
public abstract record SubjectsResponse;

/// <summary>
/// Уведомление основного чата
/// <param name="ChatId">ID чата</param>
/// <param name="SubjectId">ID уведомления</param>
/// <param name="SubjectText">Текст уведомления</param>
/// </summary>
public sealed record GeneralChatSubjectsQueryResponse(
    long ChatId,
    long SubjectId,
    string SubjectText
) : SubjectsResponse;

/// <summary>
/// Уведомление темы чата
/// <param name="ChatId">ID чата</param>
/// <param name="ThemeId">ID темы</param>
/// <param name="SubjectId">ID уведомления</param>
/// <param name="SubjectText">Текст сообщения</param>
/// </summary>
public sealed record ThemeChatSubjectsQueryResponse(
    long ChatId,
    long ThemeId,
    long SubjectId,
    string SubjectText
) : SubjectsResponse;

public static class GetSubjectsQueryResponseExtensions
{
    /// <summary>
    /// Получить ID основного чата уведомления, если уведомление основного чата
    /// <param name="response"><inheritdoc cref="GeneralChatSubjectsQueryResponse"/></param>
    /// <returns>ID основного чата уведомления</returns>
    /// <exception cref="UnreachableException">Исключение если уведомление не типа
    ///     <inheritdoc cref="GeneralChatSubjectsQueryResponse"/>
    /// </exception>
    /// </summary>
    public static long GetSubjectChatId(this SubjectsResponse response) =>
        response switch
        {
            GeneralChatSubjectsQueryResponse general => general.ChatId,
            _ => throw new UnreachableException(),
        };

    /// <summary>
    /// Получить ID темы чата
    /// <param name="response">
    ///     <inheritdoc cref="ThemeChatSubjectsQueryResponse"/>
    /// </param>
    /// <returns>
    ///     ID темы чата
    /// </returns>
    /// <exception cref="UnreachableException">
    ///     Исключение если уведомление не типа
    ///     <inheritdoc cref="ThemeChatSubjectsQueryResponse"/>
    /// </exception>
    /// </summary>
    public static long GetSubjectThemeId(this SubjectsResponse response) =>
        response switch
        {
            ThemeChatSubjectsQueryResponse theme => theme.ThemeId,
            _ => throw new UnreachableException(),
        };

    /// <summary>
    /// Получение ID уведомления
    /// <param name="response">
    /// <inheritdoc cref="SubjectsResponse"/>
    /// </param>
    /// <returns>ID уведомления</returns>
    /// <exception cref="UnreachableException">
    /// Исключение если параметр не из следующих типов:
    /// <inheritdoc cref="GeneralChatSubjectsQueryResponse"/>
    /// <inheritdoc cref="ThemeChatSubjectsQueryResponse"/>
    /// </exception>
    /// </summary>
    public static long GetSubjectId(this SubjectsResponse response) =>
        response switch
        {
            GeneralChatSubjectsQueryResponse general => general.SubjectId,
            ThemeChatSubjectsQueryResponse theme => theme.SubjectId,
            _ => throw new UnreachableException(),
        };

    /// <summary>
    /// Получение текста уведомления
    /// <param name="response">
    ///     <inheritdoc cref="SubjectsResponse"/>
    /// </param>
    /// <returns>Текст уведомления</returns>
    /// <exception cref="UnreachableException">
    /// Исключение если параметр не из следующих типов:
    /// <inheritdoc cref="GeneralChatSubjectsQueryResponse"/>
    /// <inheritdoc cref="ThemeChatSubjectsQueryResponse"/>
    /// </exception>
    /// </summary>
    public static string GetSubjectMessage(this SubjectsResponse response) =>
        response switch
        {
            GeneralChatSubjectsQueryResponse general => general.SubjectText,
            ThemeChatSubjectsQueryResponse theme => theme.SubjectText,
            _ => throw new UnreachableException(),
        };

    /// <summary>
    /// Сообщение темы или нет
    /// <param name="response"><inheritdoc cref="SubjectsResponse"/></param>
    /// <returns>True если темы, False если не темы</returns>
    /// <exception cref="UnreachableException">
    /// Исключение если параметр не из следующих типов:
    /// <inheritdoc cref="GeneralChatSubjectsQueryResponse"/>
    /// <inheritdoc cref="ThemeChatSubjectsQueryResponse"/>
    /// </exception>
    /// </summary>
    public static bool IsThemeSubject(this SubjectsResponse response) =>
        response switch
        {
            GeneralChatSubjectsQueryResponse => false,
            ThemeChatSubjectsQueryResponse => true,
            _ => throw new UnreachableException(),
        };
}
