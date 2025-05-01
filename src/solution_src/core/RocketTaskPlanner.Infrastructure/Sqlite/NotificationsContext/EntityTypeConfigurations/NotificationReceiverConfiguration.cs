using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RocketTaskPlanner.Domain.NotificationsContext;
using RocketTaskPlanner.Domain.NotificationsContext.ValueObjects;

namespace RocketTaskPlanner.Infrastructure.Sqlite.NotificationsContext.EntityTypeConfigurations;

public sealed class NotificationReceiverConfiguration
    : IEntityTypeConfiguration<NotificationReceiver>
{
    public void Configure(EntityTypeBuilder<NotificationReceiver> builder)
    {
        builder.ToTable("notification_receivers");

        builder.HasKey(r => r.Id);

        builder
            .Property(r => r.Id)
            .HasColumnName("receiver_id")
            .HasConversion(toDb => toDb.Id, fromDb => NotificationReceiverId.Create(fromDb).Value);

        builder
            .Property(r => r.Name)
            .HasColumnName("receiver_name")
            .HasConversion(
                toDb => toDb.Name,
                fromDb => NotificationReceiverName.Create(fromDb).Value
            );

        builder.ComplexProperty(
            r => r.TimeZone,
            cpb =>
            {
                cpb.Property(tz => tz.ZoneName).HasColumnName("receiver_zone_name");
                cpb.Property(tz => tz.TimeStamp).HasColumnName("receiver_zone_time_stamp");
            }
        );

        builder
            .HasMany(r => r.Themes)
            .WithOne(th => th.NotificationReceiver)
            .HasForeignKey(th => th.NotificationReceiverId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(true);

        builder
            .HasMany(r => r.Subjects)
            .WithOne(s => s.Receiver)
            .HasForeignKey(s => s.GeneralChatId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired(true);

        builder.HasIndex(r => r.Name).IsDescending();
    }
}
