using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RocketTaskPlanner.Domain.UsersContext.Entities;
using RocketTaskPlanner.Domain.UsersContext.ValueObjects;

namespace RocketTaskPlanner.Infrastructure.Sqlite.UsersContext.EntityTypeConfiguration;

/// <summary>
/// Конфигурация таблицы user_permissions (права пользователей)
/// </summary>
public sealed class UserPermissionEntityTypeConfiguration : IEntityTypeConfiguration<UserPermission>
{
    public void Configure(EntityTypeBuilder<UserPermission> builder)
    {
        builder.ToTable("user_permissions");

        builder.HasKey(p => new { p.Id, p.UserId });

        builder.Property(p => p.Id).HasColumnName("id");

        builder
            .Property(p => p.UserId)
            .HasColumnName("user_id")
            .HasConversion(toDb => toDb.Value, fromDb => UserId.Create(fromDb));

        builder.Property(p => p.Name).HasColumnName("name").IsRequired();
    }
}
