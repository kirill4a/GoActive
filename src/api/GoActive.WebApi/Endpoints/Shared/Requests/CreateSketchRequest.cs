using GoActive.Shared.Domain.Enums;
using GoActive.WebApi.Endpoints.Shared.Models;

namespace GoActive.WebApi.Endpoints.Shared.Requests;

/// <summary>
/// Request body for create object sketch
/// </summary>
public record CreateSketchRequest
{
    /// <summary>
    /// The collection of activity types
    /// </summary>
    public required IEnumerable<ActivityTypes> ActivityTypes { get; init; }

    /// <summary>
    /// Sketch title
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// Sketch location
    /// </summary>
    public required GeoLocationDto Location { get; init; }

    /// <summary>
    /// Sketch location altitude, if set
    /// </summary>
    public double? Altitude { get; init; }
}
