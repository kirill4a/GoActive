using GoActive.Modules.Geo.Domain.ValueObjects;
using GoActive.Shared.Domain;

namespace GoActive.Modules.Geo.Domain.GeoDataAggregate;

public class GeoData : EntityBase<GeoDataId>
{
    public GeoData(GeoDataId id, Title title) : base(id)
    {
        ArgumentNullException.ThrowIfNull(title);
        Title = title;
    }

    public Title Title { get; }

    public required GeoCoordinate Center { get; init; }

    public IEnumerable<GeoCoordinate> Bounds { get; init; } = [];
}