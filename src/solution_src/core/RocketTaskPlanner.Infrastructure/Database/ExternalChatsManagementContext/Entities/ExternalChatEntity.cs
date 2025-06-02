using RocketTaskPlanner.Domain.ExternalChatsManagementContext;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext.Entities;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext.ValueObjects;

namespace RocketTaskPlanner.Infrastructure.Database.ExternalChatsManagementContext.Entities;

/// <summary>
/// DAO модель чата пользователя
/// </summary>
public sealed class ExternalChatEntity
{
    public long Id { get; set; }
    public long? ParentId { get; set; }
    public string Name { get; set; } = string.Empty;
    public long OwnerId { get; set; }

    private ExternalChatEntity() { }

    public ExternalChatEntity(ExternalChatEntity entity)
    {
        Id = entity.Id;
        Name = entity.Name;
        OwnerId = entity.OwnerId;
        ParentId = entity.ParentId;
    }

    public ExternalChat ToExternalChat(ExternalChatOwner owner)
    {
        ExternalChatId id = ExternalChatId.Dedicated(Id);
        ExternalChatName name = ExternalChatName.Create(Name).Value;
        ExternalChat chat = owner.AddExternalChat(id, name).Value;

        return chat;
    }

    public ExternalChat ToExternalThemeChat(ExternalChatOwner owner, ExternalChat parent)
    {
        ExternalChatId id = ExternalChatId.Dedicated(Id);
        ExternalChatName name = ExternalChatName.Create(Name).Value;
        ExternalChat themeChat = owner.AddExternalThemeChat(id, name, parent).Value;

        return themeChat;
    }
}
