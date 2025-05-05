using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext;
using RocketTaskPlanner.Domain.ExternalChatsManagementContext.ValueObjects;

namespace RocketTaskPlanner.Infrastructure.Sqlite.ExternalChatsManagementContext.EntityTypeConfigurations;

/// <summary>
/// Конфигурация таблицы работы с БД для работы с обладателем внешнего чата
/// </summary>
public sealed class ExternalChatOwnerConfiguration : IEntityTypeConfiguration<ExternalChatOwner>
{
    public void Configure(EntityTypeBuilder<ExternalChatOwner> builder)
    {
        builder.ToTable("owners");

        builder.HasKey(o => o.Id);

        builder
            .Property(o => o.Id)
            .HasColumnName("id")
            .HasConversion(toDb => toDb.Value, fromDb => ExternalChatMemberId.Dedicated(fromDb));

        builder
            .Property(o => o.Name)
            .HasColumnName("name")
            .IsRequired()
            .HasConversion(
                toDb => toDb.Value,
                fromDb => ExternalChatMemberName.Create(fromDb).Value
            );

        builder
            .HasMany(o => o.Chats)
            .WithOne(o => o.Owner)
            .HasForeignKey(o => o.OwnerId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
