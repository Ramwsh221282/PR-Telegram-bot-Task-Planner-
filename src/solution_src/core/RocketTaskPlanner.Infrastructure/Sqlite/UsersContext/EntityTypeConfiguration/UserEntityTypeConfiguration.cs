using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RocketTaskPlanner.Domain.UsersContext;
using RocketTaskPlanner.Domain.UsersContext.ValueObjects;

namespace RocketTaskPlanner.Infrastructure.Sqlite.UsersContext.EntityTypeConfiguration;

public sealed class UserEntityTypeConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("users");

        builder.HasKey(u => u.Id);

        builder
            .Property(u => u.Id)
            .HasColumnName("id")
            .HasConversion(toDb => toDb.Value, fromDb => UserId.Create(fromDb));

        builder
            .Property(u => u.Name)
            .HasColumnName("name")
            .HasConversion(toDb => toDb.Value, fromDb => UserName.Create(fromDb).Value);

        builder
            .HasMany(u => u.Permissions)
            .WithOne(p => p.User)
            .HasForeignKey(p => p.UserId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}
