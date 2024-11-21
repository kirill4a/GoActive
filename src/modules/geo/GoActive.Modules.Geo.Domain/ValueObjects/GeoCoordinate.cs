using NetTopologySuite.Geometries;

namespace GoActive.Modules.Geo.Domain.ValueObjects;

public readonly record struct GeoCoordinate
{
    private GeoCoordinate(GeoLocation location, Altitude? altitude)
    {
        Location = location;
        Altitude = altitude;
    }

    public GeoLocation Location { get; }
    public Altitude? Altitude { get; }

    public static GeoCoordinate FromLocation(GeoLocation location) => new(location, default);
    public static GeoCoordinate FromLocationWithAltitude(GeoLocation location, Altitude altitude) => new(location, altitude);

    public Point ToPoint() => Altitude.HasValue
                                ? new(Location.Latitude.Value, Location.Longitude.Value, Altitude.Value.Value)
                                : new(Location.Latitude.Value, Location.Longitude.Value);
}