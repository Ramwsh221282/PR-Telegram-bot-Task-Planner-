using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RocketTaskPlanner.Domain.ApplicationTimeContext.Entities.TimeZones;
using RocketTaskPlanner.Domain.ApplicationTimeContext.Entities.TimeZones.ValueObjects;
using RocketTaskPlanner.Infrastructure.TimeZoneDb;

namespace RocketTaskPlanner.Infrastructure.Sqlite.ApplicationTimeContext.EntityTypeConfigurations;

public sealed class ApplicationTimeZoneConfiguration : IEntityTypeConfiguration<ApplicationTimeZone>
{
    public void Configure(EntityTypeBuilder<ApplicationTimeZone> builder)
    {
        builder.ToTable("time_zones");
        builder.HasKey(t => t.Id);

        builder
            .Property(t => t.Id)
            .HasColumnName("time_zone_id")
            .HasConversion(toDb => toDb.Id, fromDb => TimeZoneId.Create(fromDb).Value);

        builder
            .Property(t => t.ProviderId)
            .HasColumnName("provider_id")
            .HasConversion(toDb => toDb.Id, fromDb => TimeZoneDbToken.Create(fromDb).Value);

        builder
            .Property(t => t.Name)
            .HasColumnName("time_zone_name")
            .HasConversion(toDb => toDb.Name, fromDb => TimeZoneName.Create(fromDb).Value);

        builder.ComplexProperty(
            t => t.TimeInfo,
            timeInfoBuilder =>
            {
                timeInfoBuilder.Property(t => t.TimeStamp).HasColumnName("time_zone_time_stamp");
                timeInfoBuilder.Property(t => t.DateTime).HasColumnName("time_zone_date_time");
            }
        );

        builder.HasIndex(t => t.Name);
        builder.HasIndex(t => t.ProviderId);
    }
}
