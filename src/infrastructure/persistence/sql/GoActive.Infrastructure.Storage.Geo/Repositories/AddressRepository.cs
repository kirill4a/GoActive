using GoActive.Infrastructure.Storage.Geo.Entities;
using GoActive.Modules.Geo.Application.Address;

using Microsoft.Extensions.Logging;

using NetTopologySuite.Geometries;

namespace GoActive.Infrastructure.Storage.Geo.Repositories;

internal sealed class AddressRepository(IGeoContext context, ILogger<AddressRepository> logger) : IAddressCreator
{
    private readonly ILogger<AddressRepository> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    public async Task<int> UpsertAddresses(IReadOnlyCollection<CreateAddressDto> addresses, CancellationToken cancellation)
    {
        var dbEntities = addresses.Select(Map).ToList();

        const int defaultBatchSize = 2_000;

        await context.BulkInsertOrUpdateAsync(
                    dbEntities,
                    bulkAction: cfg =>
                    {
                        cfg.BatchSize = dbEntities.Count > defaultBatchSize ? dbEntities.Count : defaultBatchSize;
                        cfg.UpdateByProperties = [nameof(Address.Source), nameof(Address.ExternalId)];
                        cfg.PropertiesToExcludeOnUpdate = [nameof(Address.Id)];
                    },
                    cancellationToken: cancellation);

        return addresses.Count;
    }

    private Address Map(CreateAddressDto address)
    {
        var location = address.Location;

        if (location.Z is not Coordinate.NullOrdinate)
        {
            location = new Point(location.X, location.Y)
            {
                SRID = location.SRID,
            };

            _logger.LogWarning(
                "Altitude in address point is not supported. " +
                "Z coordinate is set to null for address with ID {@AddressId} and source is {@AddressSource}.",
                address.ExternalId,
                address.Source);
        }

        var now = DateTime.UtcNow;

        return new()
        {
            Id = Guid.NewGuid(),
            Source = address.Source,
            ExternalId = address.ExternalId,
            Country = address.CountryCode,
            Region = address.Region,
            District = address.District,
            Settlement = address.Settlement,
            Street = address.Street,
            Building = address.Building,
            PostalCode = address.PostalCode,
            Hash = address.Hash,
            Location = location,
            IsActive = true,

            CreatedAt = now,
            UpdatedAt = now,
        };
    }

    public void Dispose()
    {
        context.Dispose();
    }
}
