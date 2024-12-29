using FluentResults;

using GoActive.Modules.Geo.Domain.SpotAggregate;

using Mediator;

namespace GoActive.Modules.Geo.Application.Spot.Get;

/// <summary>
/// Query to retrieve spot
/// </summary>
/// <param name="SpotId">Spot indentifier</param>
public readonly record struct GetSpotQuery(SpotId SpotId) : IQuery<Result<GetSpotResult>>;
