using GoActive.Infrastructure.Storage.Geo.Entities;
using GoActive.Infrastructure.Storage.Geo.Extensions;
using GoActive.Shared.Domain.Enums;
using GoActive.Shared.Infrastructure;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoActive.Infrastructure.Storage.Geo.Configurations;

internal class SpotConfiguration : IEntityTypeConfiguration<Spot>
{
    public void Configure(EntityTypeBuilder<Spot> builder)
    {
        builder.ToTable("spots");

        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.Title)
            .HasMaxLength(100);

        builder
            .Property(x => x.NormalizedTitle)
            .HasMaxLength(100);

        builder
            .Property(x => x.ActivityTypes)
            .HasDefaultValue(Array.Empty<ActivityType>())
            .HasReadOnlyCollectionJsonConversion(JsonSerializerCustom.EnumSerializerOptions);

        builder
            .Property(x => x.Location)
            .HasColumnType("geometry (point)");

        builder
            .Property(x => x.Description)
            .HasMaxLength(2048);

        builder
            .HasIndex(x => x.Title)
            .HasMethod("gin")
            .HasOperators("gin_trgm_ops");

        builder
            .HasIndex(x => new { x.NormalizedTitle, x.Location })
            .IsUnique();

        builder
            .HasOne(x => x.Address)
            .WithMany()
            .HasForeignKey(x => x.AddressId);
    }
}