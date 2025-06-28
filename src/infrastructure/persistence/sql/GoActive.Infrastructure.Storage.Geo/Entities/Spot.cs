using GoActive.Shared.Domain.Enums;

using NetTopologySuite.Geometries;

namespace GoActive.Infrastructure.Storage.Geo.Entities;

/// <summary>
/// Database entity for Spot.
/// </summary>
internal class Spot : Entity
{
    /// <summary>
    /// Unique spot identifier.
    /// </summary>
    public Guid Id { get; init; }

    /// <summary>
    /// Spot title.
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// Normalized spot title for uniqueness.
    /// </summary>
    public required string NormalizedTitle { get; init; }

    /// <summary>
    /// Spot location point.
    /// </summary>
    public required Point Location { get; init; }

    /// <summary>
    /// Spot altitude (if any), in meters.
    /// </summary>
    public float? Altitude { get; init; }

    /// <summary>
    /// Spot address indentifier (if any).
    /// </summary>
    public Guid? AddressId { get; set; }

    /// <summary>
    /// Spot address (if any).
    /// </summary>
    public Address? Address { get; set; }

    /// <summary>
    /// Activities available on the spot.
    /// </summary>
    public required IReadOnlyCollection<ActivityType> Activities { get; set; }

    /// <summary>
    /// Spot description.
    /// </summary>
    public string? Description { get; set; }
}

