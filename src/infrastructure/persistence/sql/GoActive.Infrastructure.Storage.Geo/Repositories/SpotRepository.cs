using GoActive.Infrastructure.Storage.Geo.Entities;
using GoActive.Modules.Geo.Application.Spot.Search;
using GoActive.Shared.Domain.Enums;

using Microsoft.EntityFrameworkCore;

namespace GoActive.Infrastructure.Storage.Geo.Repositories;

internal class SpotRepository(IGeoContext context) : ISpotSearcher
{
    public async Task<IReadOnlyCollection<SearchSpotResult>> SearchBySpot(string queryString,
                                                                           IReadOnlyCollection<ActivityTypes> activities,
                                                                           CancellationToken cancellationToken)
    {
        var query = GetQuerable()
            .Where(x => EF.Functions.ILike(x.Title, $"%{queryString}%")
                        && (activities.Count == 0 || activities.Intersect(x.ActivityTypes).Any()))
            .Select(x => Map(x));

        return await query.ToArrayAsync(cancellationToken);
    }

    public Task<IReadOnlyCollection<SearchSpotResult>> SearchByAddress(string queryString,
                                                                       IReadOnlyCollection<ActivityTypes> activities,
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
            spot.ActivityTypes);

    private static string FlattenAddress(Address? address)
    {
        return address is null ? string.Empty
            : $"{address.Country}, {address.Region}, {address.District}, {address.Settlement}, {address.Street}, {address.Building}, {address.PostCode}";
    }

    private IQueryable<Spot> GetQuerable()
    {
        return context.Spots.AsNoTracking();
    }
}
