using RocketTaskPlanner.Domain.ExternalChatsManagementContext;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext.Entities;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext.ValueObjects;

namespace RocketTaskPlanner.Infrastructure.Database.ExternalChatsManagementContext.Entities;

/// <summary>
/// Dao модель Обладатель внешнего чата.
/// </summary>
public sealed class ExternalChatOwnerEntity
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;

    public List<ExternalChatEntity> Chats { get; set; } = [];

    private ExternalChatOwnerEntity() { }

    public ExternalChatOwnerEntity(ExternalChatOwnerEntity entity)
    {
        Id = entity.Id;
        Name = entity.Name;
    }

    public void TryAddChat(ExternalChatEntity entity)
    {
        if (Chats.Any(m => m.Id == entity.Id))
            return;

        Chats.Add(new ExternalChatEntity(entity));
    }

    public ExternalChatOwner ToExternalChatOwner()
    {
        ExternalChatMemberId id = ExternalChatMemberId.Dedicated(Id);
        ExternalChatMemberName name = ExternalChatMemberName.Create(Name).Value;
        ExternalChatOwner owner = new(id, name);

        AddExternalGeneralChats(owner);
        AddExternalThemeChats(owner);

        return owner;
    }

    private void AddExternalGeneralChats(ExternalChatOwner owner)
    {
        foreach (ExternalChatEntity entity in Chats.Where(c => c.ParentId == null))
            entity.ToExternalChat(owner);
    }

    private void AddExternalThemeChats(ExternalChatOwner owner)
    {
        foreach (ExternalChatEntity entity in Chats.Where(c => c.ParentId != null))
        {
            ExternalChatId parentId = ExternalChatId.Dedicated(entity.ParentId!.Value);
            ExternalChat parent = owner.GetParentChat(parentId).Value;
            entity.ToExternalThemeChat(owner, parent);
        }
    }
}
