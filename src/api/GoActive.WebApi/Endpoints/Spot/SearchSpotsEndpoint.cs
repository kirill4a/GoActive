using System.Net;
using GoActive.Shared.Domain.Enums;
using GoActive.Modules.Geo.Application.Spot;
using GoActive.Modules.Geo.Application.Spot.Search;
using GoActive.WebApi.Infrastructure.Endpoints;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using GoActive.WebApi.Infrastructure;

namespace GoActive.WebApi.Endpoints.Spot;

internal class SearchSpotsEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapGet("spots",
                   async Task<Results<ProblemHttpResult, Ok<SpotDto[]>>> ([FromQuery(Name = "q")] string queryText,
                                                                          [FromQuery] ActivityTypes[]? activities,
                                                                          ISender sender,
                                                                          CancellationToken cancellation) =>
        {
            if (string.IsNullOrWhiteSpace(queryText) && (activities is null || activities.Length == 0))
            {
                return TypedResults.Ok(Array.Empty<SpotDto>());
            }

            var query = new SearchSpotsQuery(queryText, activities ?? []);
            var result = await sender.Send(query, cancellation);

            if (result.IsFailed)
                return TypedResults.Problem();

            return TypedResults.Ok(result.Value);
        })
        .ProducesProblem((int)HttpStatusCode.InternalServerError)
        .WithOpenApi(options => new(options)
        {
            OperationId = "SearchSpots",
            Description = "Use this method to search spots",
            Tags = [new OpenApiTag() { Name = "Spots" }],
            Summary = "Retrieves list of founded spots",
        })
        .WithName(Constants.Routes.SearchSpots);
}
