using System.Net;
using Mediator;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;
using GoActive.Modules.Geo.Application.Spot.Get;
using GoActive.Modules.Geo.Domain.SpotAggregate;
using GoActive.WebApi.Infrastructure;
using GoActive.WebApi.Infrastructure.Endpoints;

namespace GoActive.WebApi.Endpoints.Spot;

internal sealed class GetSpotEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
    app.MapGet("spots/{spotId}",
                async Task<Results<ProblemHttpResult, Ok<GetSpotResult>>> ([FromRoute] Guid spotId,
                                                                           ISender sender,
                                                                           CancellationToken cancellation) =>
    {
        var query = new GetSpotQuery(SpotId.FromValue(spotId));
        var result = await sender.Send(query, cancellation);

        if (result.IsFailed)
            return TypedResults.Problem();

        return TypedResults.Ok(result.Value);
    })
    .ProducesProblem((int)HttpStatusCode.InternalServerError)
    .WithOpenApi(options => new(options)
    {
        OperationId = "GetSpot",
        Description = "Use this method to get spot",
        Tags = [new OpenApiTag() { Name = "Spots" }],
        Summary = "Get detailed spot info",
    })
    .WithName(Constants.Routes.GetSpot);
}
