using GoActive.Shared.Domain.Enums;

namespace GoActive.Modules.Geo.Application.Spot.Create;

/// <summary>
/// Create spot DTO.
/// </summary>
public sealed record CreateSpotDto
{
    /// <summary>
    /// Spot title.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// Spot latitude.
    /// </summary>
    public double Latitude { get; init; }

    /// <summary>
    /// Spot longitude.
    /// </summary>
    public double Longitude { get; init; }

    /// <summary>
    /// Spot altitude.
    /// </summary>
    public double? Altitude { get; init; }

    /// <summary>
    /// Spot activities.
    /// </summary>
    public required IReadOnlyCollection<ActivityTypes> Activities { get; init; }

    /// <summary>
    /// Spot description.
    /// </summary>
    public string? Description { get; init; }

    /// <summary>
    /// Spot address identifier.
    /// </summary>
    public Guid? AddressId { get; init; }
}
