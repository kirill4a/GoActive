using GoActive.Infrastructure.Storage.Geo.Entities;
using GoActive.Modules.Geo.Application.Spot.Create;
using GoActive.Modules.Geo.Application.Spot.Search;
using GoActive.Shared.Domain.Enums;

using Microsoft.EntityFrameworkCore;

using DomainAddress = GoActive.Modules.Geo.Domain.ValueObjects.Address;
using DomainSpot = GoActive.Modules.Geo.Domain.SpotAggregate.Spot;

namespace GoActive.Infrastructure.Storage.Geo.Repositories;

internal class SpotRepository(IGeoContext context) : ISpotSearcher, ISpotCreator
{
    public async Task<IReadOnlyCollection<SearchSpotResult>> SearchBySpot(string queryString,
                                                                           IReadOnlyCollection<ActivityType> activities,
                                                                           CancellationToken cancellationToken)
    {
        var query = GetQuerable()
            .Where(x => EF.Functions.ILike(x.Title, $"%{queryString}%")
                        && (activities.Count == 0 || activities.Intersect(x.ActivityTypes).Any()))
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

    public void CreateSpots(IReadOnlyCollection<DomainSpot> spots)
    {
        var dbEntities = spots.Select(Map);
        context.Spots.AddRange(dbEntities);
    }

    private static SearchSpotResult Map(Spot spot)
        =>
        new(spot.Id,
            new(spot.Location.X, spot.Location.Y),
            spot.Title,
            FlattenAddress(spot.Address),
            spot.ActivityTypes);

    private static Spot Map(DomainSpot spot)
        =>
        new()
        {
            Id = spot.Id.Value,
            Title = spot.Title.Value,
            Location = spot.LocationPoint.Altitude.HasValue
                ? new(spot.LocationPoint.Location.Latitude.Value, spot.LocationPoint.Location.Longitude.Value, spot.LocationPoint.Altitude.Value.Value)
                : new(spot.LocationPoint.Location.Latitude.Value, spot.LocationPoint.Location.Longitude.Value),
            AddressId = spot.AddressId?.Value,
            ActivityTypes = spot.Activities,
            Description = spot.Description,
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
