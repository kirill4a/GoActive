using GoActive.Modules.Geo.Application.Shared.Dto;
using GoActive.Shared.Domain.Enums;

namespace GoActive.Modules.Geo.Application.Spot.Locate;

/// <summary>
/// The item of locating spot result.
/// </summary>
/// <param name="Id">Spot identidier.</param>
/// <param name="Location">Spot location.</param>
/// <param name="Title">Spot title.</param>
/// <param name="Activities">Activities supported by the spot.</param>
public record LocateSpotResult(Guid Id, GeoLocationDto Location, string Title, IEnumerable<ActivityType> Activities);
