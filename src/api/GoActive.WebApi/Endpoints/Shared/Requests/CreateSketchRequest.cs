using GoActive.Shared.Domain.Enum;
using GoActive.WebApi.Endpoints.Shared.Models;

namespace GoActive.WebApi.Endpoints.Shared.Requests;

/// <summary>
/// Request body for create object sketch
/// </summary>
public record CreateSketchRequest
{
    /// <summary>
    /// The type of activity
    /// </summary>
    public required ActivityTypes ActivityType { get; init; }

    /// <summary>
    /// Object title
    /// </summary>
    public required string Title { get; init; }

    /// <summary>
    /// Object loaction
    /// </summary>
    public required GeoLocationDto Location { get; init; }
}
