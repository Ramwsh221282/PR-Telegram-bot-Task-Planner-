using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RocketTaskPlanner.Domain.PermissionsContext;

namespace RocketTaskPlanner.Infrastructure.Sqlite.PermissionsContext.EntityTypeConfigurations;

/// <summary>
/// Настройка таблицы прав
/// </summary>
public sealed class PermissionEntityTypeConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions");

        builder.HasKey(p => p.Id);

        builder.Property(p => p.Id).HasColumnName("id");

        builder.Property(p => p.Name).HasColumnName("name").IsRequired();
    }
}
