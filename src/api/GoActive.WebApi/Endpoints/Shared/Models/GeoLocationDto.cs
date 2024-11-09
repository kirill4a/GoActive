namespace GoActive.WebApi.Endpoints.Shared.Models;

/// <summary>
/// Represents the coordinate point on Earth surface
/// </summary>
/// <param name="Latitude">The latitude in range of -90.0 and 90.0</param>
/// <param name="Longitude">The longitude in range of -180.0 and 180.0</param>
public readonly record struct GeoLocationDto(double Latitude, double Longitude);
