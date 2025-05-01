using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RocketTaskPlanner.Domain.PermissionsContext;

namespace RocketTaskPlanner.Infrastructure.Sqlite.PermissionsContext.EntityTypeConfigurations;

public sealed class PermissionEntityTypeConfiguration : IEntityTypeConfiguration<Permission>
{
    public void Configure(EntityTypeBuilder<Permission> builder)
    {
        builder.ToTable("permissions");
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired();
    }
}
