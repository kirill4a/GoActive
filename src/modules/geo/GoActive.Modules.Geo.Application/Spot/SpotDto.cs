using GoActive.Modules.Geo.Application.Shared.Dto;
using GoActive.Shared.Domain.Enums;

namespace GoActive.Modules.Geo.Application.Spot;

/// <summary>
/// The spot of sport activity object.
/// </summary>
public sealed record SpotDto
{
    /// <summary>
    /// Spot identidier
    /// </summary>
    public required Guid Id { get; init; }

    /// <summary>
    /// Spot title
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// Activities supported by the spot
    /// </summary>
    public required IEnumerable<ActivityTypes> Activities { get; init; }

    /// <summary>
    /// Spot location
    /// </summary>
    public GeoLocationDto? Location { get; init; }

    /// <summary>
    /// Spot address
    /// </summary>
    public AddressDto? Address { get; init; }

    /// <summary>
    /// The spot description
    /// </summary>
    public string? Description { get; init; }
}
