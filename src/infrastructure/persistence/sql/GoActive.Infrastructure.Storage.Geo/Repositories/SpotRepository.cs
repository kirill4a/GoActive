using GoActive.Infrastructure.Storage.Geo.Entities;
using GoActive.Modules.Geo.Application.Spot.Create;
using GoActive.Modules.Geo.Application.Spot.Search;
using GoActive.Shared.Domain.Enums;

using Microsoft.EntityFrameworkCore;

using DomainAddress = GoActive.Modules.Geo.Domain.ValueObjects.Address;
using DomainSpot = GoActive.Modules.Geo.Domain.SpotAggregate.Spot;
using DomainSpotKey = GoActive.Modules.Geo.Domain.SpotAggregate.SpotKey;

namespace GoActive.Infrastructure.Storage.Geo.Repositories;

internal class SpotRepository(IGeoContext context) : ISpotSearcher, ISpotCreator
{
    public async Task<bool> ExistsAsync(DomainSpotKey key, CancellationToken cancellationToken)
    {
        var (normalizedTitle, location) = (key.NormalizedTitle.Value, key.LocationPoint.ToPoint());
        return await context.Spots.AnyAsync(x => x.NormalizedTitle == normalizedTitle && x.Location == location, cancellationToken);
    }

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
        var query = GetQuerable()
            .Where(x => EF.Functions.ILike(x.Title, $"%{queryString}%")
                        && (activities.Count == 0 || activities.Intersect(x.Activities).Any()))
            .Select(x => Map(x));

        return await query.ToArrayAsync(cancellationToken);
    }

    public Task<IReadOnlyCollection<SearchSpotResult>> SearchByAddress(string queryString,
                                                                       IReadOnlyCollection<ActivityType> activities,
                                                                       CancellationToken cancellationToken)
    {
        // TODO: Implement search by address
        return Task.FromResult(Array.Empty<SearchSpotResult>() as IReadOnlyCollection<SearchSpotResult>);
    }

    private static SearchSpotResult Map(Spot spot)
        =>
        new(spot.Id,
            new(spot.Location.X, spot.Location.Y),
            spot.Title,
            FlattenAddress(spot.Address),
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

    private IQueryable<Spot> GetQuerable()
    {
        return context.Spots.AsNoTracking();
    }
}
