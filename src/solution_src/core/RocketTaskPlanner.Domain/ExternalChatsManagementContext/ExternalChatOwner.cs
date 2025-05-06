using RocketTaskPlanner.Domain.ExternalChatsManagementContext.Entities;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext.ValueObjects;

namespace RocketTaskPlanner.Domain.ExternalChatsManagementContext;

/// <summary>
/// Обладатель внешнего чата.
/// </summary>
public sealed class ExternalChatOwner
{
    /// <summary>
    /// Список чатов обладателя внешних чатов
    /// </summary>
    private readonly List<ExternalChat> _chats = [];

    /// <summary>
    /// ID обладателя внешнего чата
    /// </summary>
    public ExternalChatMemberId Id { get; }

    /// <summary>
    /// Имя обладателя внешнего чата
    /// </summary>
    public ExternalChatMemberName Name { get; private set; } = null!;

    /// <summary>
    /// Список чатов обладателя внешних чатов для чтения.
    /// </summary>
    public IReadOnlyCollection<ExternalChat> Chats => _chats;

    private ExternalChatOwner() { } // ef core

    public ExternalChatOwner(ExternalChatMemberId id, ExternalChatMemberName name)
    {
        Id = id;
        Name = name;
    }

    /// <summary>
    /// Добавление внешнего чата обладателю. Если чат с id уже присутствует, возврат ошибки.
    /// </summary>
    /// <param name="id">id внешнего чата</param>
    /// <param name="name">имя внешнего чата</param>
    /// <returns>ExternalChat. Success если чат был добавлен. Failure если чат не был добавлен</returns>
    public Result<ExternalChat> AddExternalChat(ExternalChatId id, ExternalChatName name)
    {
        if (_chats.Any(c => c.Id == id))
            return Result.Failure<ExternalChat>($"Чат с id: {id.Value} уже добавлен.");

        ExternalChat chat = new(this, id, name);
        _chats.Add(chat);

        return chat;
    }

    /// <summary>
    /// Добавление темы чата обладателю. Если эта тема уже присутствует, возврат ошибки.
    /// </summary>
    /// <param name="themeChatId">ID темы чата</param>
    /// <param name="parent">Основной чат</param>
    /// /// <param name="themeChatName">Название темы чата</param>
    /// <returns></returns>
    public Result<ExternalChat> AddExternalThemeChat(
        ExternalChatId themeChatId,
        ExternalChatName themeChatName,
        ExternalChat parent
    )
    {
        if (_chats.Any(c => c.Id == themeChatId && c.ParentId == parent.Id))
            return Result.Failure<ExternalChat>(
                $"Тема с id: {themeChatId.Value} чата: {parent.Id.Value} уже добавлена"
            );

        ExternalChat themeChat = new(this, parent, themeChatId, themeChatName);
        _chats.Add(themeChat);

        return themeChat;
    }

    /// <summary>
    /// Получить основной чат по ID
    /// </summary>
    /// <param name="id">ID основного чата</param>
    /// <returns>Success External Chat, если существует с таким ID. Failure если не найден.</returns>
    public Result<ExternalChat> GetParentChat(ExternalChatId id)
    {
        var parent = _chats.FirstOrDefault(c => c.Id == id);
        return parent ?? Result.Failure<ExternalChat>("Основной чат не найден.");
    }

    public bool OwnsChat(long chatId) => _chats.Any(c => c.Id.Value == chatId);

    /// <summary>
    /// Удаление внешнего чата обладателя. Если чат не был найден - возврат ошибки.
    /// </summary>
    /// <param name="id">Id внешнего чата</param>
    /// <returns>ExternalChat. Success если чат был удалён. Failure если чат не был найден</returns>
    public Result<ExternalChat> RemoveExternalChat(ExternalChatId id)
    {
        var chat = _chats.FirstOrDefault(c => c.Id == id);
        if (chat == null)
            return Result.Failure<ExternalChat>($"Чат с id: {id.Value} не найден");

        _chats.Remove(chat);
        return chat;
    }

    /// <summary>
    /// Получение дочернего чата по его ID и ID родителя.
    /// </summary>
    /// <param name="parentId">ID родителя</param>
    /// <param name="childId">ID дочернего чата</param>
    /// <returns>External Chat. Success если дочерний чат был найден. Failure если чат не был найден</returns>
    public Result<ExternalChat> GetChildChat(ExternalChatId parentId, ExternalChatId childId)
    {
        var chat = _chats.FirstOrDefault(c =>
            c.ParentId != null && c.ParentId == parentId && c.Id == childId
        );

        return chat ?? Result.Failure<ExternalChat>("Не найден дочерний чат.");
    }

    public ExternalChat[] GetGeneralChats() => [.. _chats.Where(c => c.ParentId == null)];

    public ExternalChat[] GetChildChats() => [.. _chats.Where(c => c.ParentId != null)];
}
