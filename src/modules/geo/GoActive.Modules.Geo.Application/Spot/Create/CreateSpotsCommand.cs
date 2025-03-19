using Mediator;

namespace GoActive.Modules.Geo.Application.Spot.Create;

/// <summary>
/// Create spot command.
/// </summary>
/// <param name="SpotDtos">Collection of spots to create.</param>
public sealed record CreateSpotsCommand(IReadOnlyCollection<CreateSpotDto> SpotDtos) : ICommand;
