namespace GoActive.Modules.Geo.Domain.ValueObjects;

// TODO: consider using NetTopologySuite package for spatial and geometry data (if any calculations will be required)
public readonly record struct GeoLocation
{
    private GeoLocation(Latitude latitude, Longitude longitude)
    {
        Latitude = latitude;
        Longitude = longitude;
    }

    public Latitude Latitude { get; }
    public Longitude Longitude { get; }

    public static GeoLocation FromLatLon(double latitude, double longitude) => new(new(latitude), new(longitude));
}
