using GoActive.Infrastructure.Storage.Geo.Entities;
using GoActive.Infrastructure.Storage.Geo.Extensions;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace GoActive.Infrastructure.Storage.Geo.Configurations;

internal class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("addresses");

        builder.HasKey(x => x.Id);

        builder
            .Property(x => x.Source)
            .HasEnumConversion();

        builder
            .Property(x => x.ExternalId)
            .HasMaxLength(31);

        builder
            .Property(x => x.Country)
            .HasMaxLength(2)
            .HasColumnType("char(2)");

        builder
            .Property(x => x.Region)
            .HasMaxLength(128);

        builder
            .Property(x => x.District)
            .HasMaxLength(128);

        builder
            .Property(x => x.Settlement)
            .HasMaxLength(82);

        builder
            .Property(x => x.Street)
            .HasMaxLength(128);

        builder
            .Property(x => x.Building)
            .HasMaxLength(17);

        builder
            .Property(x => x.PostCode)
            .HasMaxLength(17);

        builder
            .Property(x => x.Hash)
            .HasMaxLength(64);

        builder
            .Property(x => x.IsActive)
            .HasDefaultValue(true);

        builder
            .Property(x => x.Location)
            .HasColumnType("geometry (point)");

        builder
            .HasIndex(x => x.Country);
    }
}
