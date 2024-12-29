using GoActive.Modules.Geo.Application.Spot.Search;

namespace GoActive.WebApi.Endpoints.Spot.Responses;

/// <summary>
/// Response of searching spots.
/// </summary>
/// <param name="Items">Founded spots.</param>
public record SearchSpotsResponse(IEnumerable<SearchSpotResult> Items);
