using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects;
using RocketTaskPlanner.Domain.NotificationsContext.Entities.ReceiverSubjects.ValueObjects;

namespace RocketTaskPlanner.Infrastructure.Database.NotificationsContext.EntityTypeConfigurations;

/// <summary>
/// Настройка таблицы для модели уведомления темы чата
/// </summary>
public sealed class ThemeChatSubjectConfiguration : IEntityTypeConfiguration<ThemeChatSubject>
{
    public void Configure(EntityTypeBuilder<ThemeChatSubject> builder)
    {
        builder.ToTable("theme_chat_subjects");
        builder.HasKey(s => s.Id);
        builder
            .Property(s => s.Id)
            .HasColumnName("theme_chat_subject_id")
            .HasConversion(toDb => toDb.Id, fromDb => ReceiverSubjectId.Create(fromDb).Value);

        builder.Property(s => s.ThemeId).HasColumnName("theme_id");

        builder
            .Property(s => s.Message)
            .HasColumnName("subject_message")
            .HasConversion(
                toDb => toDb.Message,
                fromDb => ReceiverSubjectMessage.Create(fromDb).Value
            );

        builder.ComplexProperty(
            s => s.TimeInfo,
            cpb =>
            {
                cpb.Property(ti => ti.Created)
                    .HasColumnName("subject_created")
                    .HasConversion(
                        toDb => toDb.Value,
                        fromDb => new ReceiverSubjectDateCreated(fromDb)
                    );

                cpb.Property(ti => ti.Notify)
                    .HasColumnName("subject_notify")
                    .HasConversion(
                        toDb => toDb.Value,
                        fromDb => new ReceiverSubjectDateNotify(fromDb)
                    );
            }
        );

        builder.ComplexProperty(
            s => s.Period,
            cpb =>
            {
                cpb.Property(pi => pi.IsPeriodic).HasColumnName("subject_periodic");
            }
        );

        builder.HasIndex(s => s.Message).IsDescending();
        builder.HasIndex(s => s.ThemeId).IsDescending();
    }
}
