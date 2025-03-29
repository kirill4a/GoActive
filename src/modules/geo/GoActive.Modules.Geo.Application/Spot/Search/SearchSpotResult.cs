using GoActive.Modules.Geo.Application.Shared.Dto;
using GoActive.Shared.Domain.Enums;

namespace GoActive.Modules.Geo.Application.Spot.Search;

/// <summary>
/// The item of searching spot result.
/// </summary>
/// <param name="Id">Spot identidier.</param>
/// <param name="Location">Spot location.</param>
/// <param name="Title">Spot title.</param>
/// <param name="Address">Spot address.</param>
/// <param name="Activities">Activities supported by the spot.</param>
public record SearchSpotResult(Guid Id, GeoLocationDto Location, string Title, string? Address, IEnumerable<ActivityType> Activities);
