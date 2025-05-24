using GoActive.Infrastructure.Storage.Geo.Entities;
using GoActive.Modules.Geo.Application.Address;

namespace GoActive.Infrastructure.Storage.Geo.Repositories;

internal sealed class AddressRepository(IGeoContext context) : IAddressCreator
{
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

    private static Address Map(CreateAddressDto address)
    {
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
            Location = address.Location,
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
