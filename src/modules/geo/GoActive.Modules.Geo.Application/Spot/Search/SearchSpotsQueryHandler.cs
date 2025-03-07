using FluentResults;

using Mediator;

namespace GoActive.Modules.Geo.Application.Spot.Search;

public class SearchSpotsQueryHandler(ISpotSearcher spotSearcher) : IQueryHandler<SearchSpotsQuery, Result<SearchSpotResult[]>>
{
    public async ValueTask<Result<SearchSpotResult[]>> Handle(SearchSpotsQuery query, CancellationToken cancellationToken)
    {
        query.Deconstruct(out var queryString, out var activities);

        var searchBySpotTask = spotSearcher.SearchBySpot(queryString, activities, cancellationToken);
        var searchByAddressTask = spotSearcher.SearchByAddress(queryString, activities, cancellationToken);

        await Task.WhenAll(searchBySpotTask, searchByAddressTask);

        var searchResult = searchBySpotTask.Result.Intersect(searchByAddressTask.Result);
        return Result.Ok(searchResult.ToArray());
    }
}
