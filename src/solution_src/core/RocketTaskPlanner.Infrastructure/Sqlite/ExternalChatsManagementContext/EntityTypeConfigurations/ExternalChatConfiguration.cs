using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext.Entities;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext.ValueObjects;

namespace RocketTaskPlanner.Infrastructure.Sqlite.ExternalChatsManagementContext.EntityTypeConfigurations;

/// <summary>
/// Модель таблицы БД для внешего чата.
/// </summary>
public sealed class ExternalChatConfiguration : IEntityTypeConfiguration<ExternalChat>
{
    public void Configure(EntityTypeBuilder<ExternalChat> builder)
    {
        builder.ToTable("external_chats");

        builder.HasKey(c => c.Id);

        builder
            .Property(c => c.Id)
            .HasColumnName("id")
            .HasConversion(toDb => toDb.Value, fromDb => ExternalChatId.Dedicated(fromDb));

        builder
            .Property(c => c.ParentId)
            .HasColumnName("parent_id")
            .IsRequired(false)
            .HasConversion(toDb => toDb!.Value.Value, fromDb => ExternalChatId.Dedicated(fromDb));

        builder
            .Property(c => c.OwnerId)
            .HasColumnName("owner_id")
            .HasConversion(toDb => toDb.Value, fromDb => ExternalChatMemberId.Dedicated(fromDb));

        builder
            .Property(c => c.Name)
            .HasColumnName("name")
            .HasConversion(toDb => toDb.Value, fromDb => ExternalChatName.Create(fromDb).Value);
    }
}
