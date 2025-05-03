using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using RocketTaskPlanner.Infrastructure.TimeZoneDb;

namespace RocketTaskPlanner.Infrastructure.Sqlite.ApplicationTimeContext.EntityTypeConfigurations;

/// <summary>
/// Конфигурация модели таблицы провайдера временной зоны в БД
/// </summary>
public sealed class TimeZoneDbProviderConfiguration : IEntityTypeConfiguration<TimeZoneDbProvider>
{
    public void Configure(EntityTypeBuilder<TimeZoneDbProvider> builder)
    {
        builder.ToTable("time_zone_db_providers");
        builder.HasKey(t => t.Id);

        builder
            .Property(t => t.Id)
            .HasColumnName("time_zone_db_provider_id")
            .HasConversion(toDb => toDb.Id, fromDb => TimeZoneDbToken.Create(fromDb).Value);

        builder
            .HasMany(t => t.TimeZones)
            .WithOne(t => t.Provider as TimeZoneDbProvider)
            .HasForeignKey(t => t.ProviderId)
            .OnDelete(DeleteBehavior.Cascade)
            .IsRequired();
    }
}
