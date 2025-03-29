using FluentResults;

using GoActive.Shared.Domain.Enums;

using Mediator;

namespace GoActive.Modules.Geo.Application.Spot.Search;

/// <summary>
/// Query to search spots by text
/// </summary>
/// <param name="Query">Query string to search</param>
/// <param name="ActivityTypes">Activities to search</param>
public record SearchSpotsQuery(string Query, params ActivityType[] ActivityTypes)
    : IQuery<Result<SearchSpotResult[]>>;
