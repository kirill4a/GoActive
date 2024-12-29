using FluentResults;

using GoActive.Shared.Domain.Enums;
using GoActive.Shared.Domain.Extensions;

using Mediator;

namespace GoActive.Modules.Geo.Application.Spot.Search;

public class SearchSpotsQueryHandler : IQueryHandler<SearchSpotsQuery, Result<SearchSpotResult[]>>
{
    private static readonly Func<Domain.SpotAggregate.Spot, string, bool> TextPredicate = (x, text) =>
        x.Title.Value.Contains(text, StringComparison.OrdinalIgnoreCase) ||
        (x.Address?.Contains(text) ?? false);

    private static readonly Func<Domain.SpotAggregate.Spot, ActivityTypes[], bool> ActivitiesPredicate = (x, activities) =>
        activities.Length == 0 || activities.Any(a => x.Activities.HasFlag(a));

    public ValueTask<Result<SearchSpotResult[]>> Handle(SearchSpotsQuery query, CancellationToken cancellationToken)
    {
        query.Deconstruct(out var queryString, out var activities);

#pragma warning disable CS0618 // Type or member is obsolete
        var searchResult = Domain.SpotAggregate.Spot.InitialData
                                .Where(x => TextPredicate(x, queryString) && ActivitiesPredicate(x, activities))
                                .Select(x => new SearchSpotResult(x.Id.Value,
                                                                  x.Title.Value,
                                                                  x.Activities.FlagsToArray()))
                                .ToArray();
#pragma warning restore CS0618 // Type or member is obsolete

        return ValueTask.FromResult(Result.Ok(searchResult));
    }
}
