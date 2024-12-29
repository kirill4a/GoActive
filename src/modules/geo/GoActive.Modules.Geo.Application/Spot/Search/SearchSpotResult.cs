using GoActive.Shared.Domain.Enums;

namespace GoActive.Modules.Geo.Application.Spot.Search;

/// <summary>
/// The item of searching spot result.
/// </summary>
/// <param name="Id">Spot identidier.</param>
/// <param name="Title">Spot title.</param>
/// <param name="Activities">Activities supported by the spot.</param>
public record SearchSpotResult(Guid Id, string Title, IEnumerable<ActivityTypes> Activities);
