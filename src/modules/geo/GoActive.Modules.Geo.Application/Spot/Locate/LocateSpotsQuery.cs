using FluentResults;

using GoActive.Modules.Geo.Application.Shared.Dto;
using GoActive.Shared.Domain.Enums;

using Mediator;

namespace GoActive.Modules.Geo.Application.Spot.Locate;

/// <summary>
/// Query to search spots by map
/// </summary>
/// <param name="MapRegion">Map boundary to search within</param>
/// <param name="ActivityTypes">Activities to search</param>
public record class LocateSpotsQuery(IEnumerable<GeoLocationDto> MapRegion, params ActivityTypes[] ActivityTypes)
    : IQuery<Result<LocateSpotResult[]>>;
