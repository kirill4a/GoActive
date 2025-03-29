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
    public Guid Id { get; set; }

    /// <summary>
    /// Spot title.
    /// </summary>
    public required string Title { get; set; }

    /// <summary>
    /// Spot location point.
    /// </summary>
    public required Point Location { get; set; }

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
    public required IReadOnlyCollection<ActivityType> ActivityTypes { get; set; }

    /// <summary>
    /// Spot description.
    /// </summary>
    public string? Description { get; set; }
}

