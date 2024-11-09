namespace GoActive.Modules.Geo.Domain.ValueObjects;

//TODO: consider using NetTopologySuite package for spatial and geometry data (if any calculations will be required)
public readonly record struct GeoLocation(Latitude Latitude, Longitude Longitude);
