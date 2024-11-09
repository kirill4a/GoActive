using NetTopologySuite.Geometries;

namespace GoActive.Modules.Geo.Domain.ValueObjects;

public readonly record struct GeoCoordinate(GeoLocation Location, Altitude Altitude)
{
    public Point ToPoint() => new(Location.Latitude.Value, Location.Longitude.Value, Altitude.Value);
}
