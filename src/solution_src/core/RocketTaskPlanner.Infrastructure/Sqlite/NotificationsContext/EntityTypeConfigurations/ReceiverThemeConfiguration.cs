using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverThemes;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverThemes.ValueObjects;

namespace RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.EntityTypeConfigurations;

public sealed class ReceiverThemeConfiguration : IEntityTypeConfiguration<ReceiverTheme>
{
    public void Configure(EntityTypeBuilder<ReceiverTheme> builder)
    {
        builder.ToTable("receiver_themes");

        builder.HasKey(th => th.Id);

        builder
            .Property(th => th.Id)
            .HasColumnName("theme_id")
            .HasConversion(toDb => toDb.Id, fromDb => ReceiverThemeId.Create(fromDb).Value);

        builder.Property(th => th.NotificationReceiverId).HasColumnName("receiver_id");

        builder
            .HasMany(th => th.Subjects)
            .WithOne(s => s.Theme)
            .HasForeignKey(s => s.ThemeId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(true);

        builder.HasIndex(th => th.NotificationReceiverId).IsDescending();
    }
}
