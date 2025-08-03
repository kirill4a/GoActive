using GoActive.Infrastructure.Storage.Geo.Entities;
using GoActive.Modules.Geo.Application.Shared.Dto;
using GoActive.Modules.Geo.Application.Spot.Create;
using GoActive.Modules.Geo.Application.Spot.Get;
using GoActive.Modules.Geo.Application.Spot.Search;
using GoActive.Shared.Domain.Enums;

using Microsoft.EntityFrameworkCore;

using DomainAddress = GoActive.Modules.Geo.Domain.ValueObjects.Address;
using DomainSpot = GoActive.Modules.Geo.Domain.SpotAggregate.Spot;
using DomainSpotId = GoActive.Modules.Geo.Domain.SpotAggregate.SpotId;
using DomainSpotKey = GoActive.Modules.Geo.Domain.SpotAggregate.SpotKey;

namespace GoActive.Infrastructure.Storage.Geo.Repositories;

internal class SpotRepository(IGeoContext context) : ISpotSearcher, ISpotCreator
{
    public async Task<bool> ExistsAsync(DomainSpotKey key, CancellationToken cancellationToken)
    {
        var (normalizedTitle, location) = (key.NormalizedTitle.Value, key.LocationPoint.ToPoint());
        return await context.Spots.AnyAsync(x => x.NormalizedTitle == normalizedTitle && x.Location == location, cancellationToken);
    }

    public Task<GetSpotResult?> GetAsync(DomainSpotId id, CancellationToken cancellationToken)
        =>
        context.Spots
            .Where(x => x.Id == id.Value)
            .Select(x => new GetSpotResult
            {
                Id = x.Id,
                Title = x.Title,
                Location = new(x.Location.X, x.Location.Y),
                Activities = x.Activities,
                Address = x.AddressId == null ? null
                                : new AddressDto
                                {
                                    Country = x.Address!.Country,
                                    Region = x.Address.Region,
                                    Settlement = x.Address.Settlement,
                                    Street = x.Address.Street,
                                    Building = x.Address.Building,
                                },
                Description = x.Description,
            })
            .FirstOrDefaultAsync(cancellationToken);

    public async Task<int> BulkInsertSpotsAsync(IReadOnlyCollection<DomainSpot> spots, CancellationToken cancellationToken)
    {
        const int defaultBatchSize = 2_000;

        var dbEntities = spots.Select(Map).ToList();

        var stats = await context.BulkInsertAsync(
                                        dbEntities,
                                        bulkAction: cfg =>
                                        {
                                            cfg.BatchSize = dbEntities.Count > defaultBatchSize ? dbEntities.Count : defaultBatchSize;
                                            cfg.UpdateByProperties = [];
                                            cfg.PropertiesToExcludeOnUpdate = [];
                                        },
                                        cancellationToken: cancellationToken);

        return stats.StatsNumberInserted;
    }

    public async Task<IReadOnlyCollection<SearchSpotResult>> SearchBySpot(string queryString,
                                                                          IReadOnlyCollection<ActivityType> activities,
                                                                          CancellationToken cancellationToken)
    {
        var query = context.Spots.AsNoTracking()
            .Include(x => x.Address).AsNoTracking()
            .Where(x => EF.Functions.ILike(x.Title, $"%{queryString}%")
                        && (activities.Count == 0 || activities.Intersect(x.Activities).Any()))
            .Select(x => Map(x, x.Address));

        return await query.ToArrayAsync(cancellationToken);
    }

    public async Task<IReadOnlyCollection<SearchSpotResult>> SearchByAddress(string queryString,
                                                                       IReadOnlyCollection<ActivityType> activities,
                                                                       CancellationToken cancellationToken)
    {
        var query =
            from address in context.Addresses.AsNoTracking()
            where address.Settlement != null && EF.Functions.ILike(address.Settlement, $"%{queryString}%")
            join spot in context.Spots.AsNoTracking() on address.Id equals spot.AddressId
            where activities.Count == 0 || activities.Intersect(spot.Activities).Any()
            select Map(spot, address);

        return await query.ToArrayAsync(cancellationToken);
    }

    private static SearchSpotResult Map(Spot spot, Address? address)
        =>
        new(spot.Id,
            new(spot.Location.X, spot.Location.Y),
            spot.Title,
            FlattenAddress(address),
            spot.Activities);

    private static Spot Map(DomainSpot spot)
        =>
        new()
        {
            Id = spot.Id.Value,
            Title = spot.Title.Value,
            NormalizedTitle = spot.Key.NormalizedTitle.Value,
            Location = new(
                spot.LocationPoint.Location.Latitude.Value,
                spot.LocationPoint.Location.Longitude.Value),
            Altitude = spot.LocationPoint.Altitude?.Value,
            AddressId = spot.AddressId?.Value,
            Activities = spot.Activities,
            Description = spot.Description,

            CreatedAt = spot.CreatedAt,
            UpdatedAt = spot.UpdatedAt,
        };

    private static string FlattenAddress(Address? address)
        =>
        address is null ? string.Empty
        : DomainAddress.Create(address.Country, address.Region, address.Settlement, address.Street, address.Building, address.PostalCode).ToString();
}
