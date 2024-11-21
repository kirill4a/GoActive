using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.OpenApi.Models;
using MediatR;
using GoActive.Modules.Geo.Application.Sketch;
using GoActive.WebApi.Infrastructure.Endpoints;
using GoActive.Modules.Geo.Application.Sketch.Get;

namespace GoActive.WebApi.Endpoints.Shared;

internal class GetSketchEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
    app.MapGet("sketches/{sketchId}",
                async Task<Results<ProblemHttpResult, NotFound, Ok<SketchDto>>> (Guid sketchId,
                                                                                 ISender sender,
                                                                                 CancellationToken cancellation) =>
                {
                    var query = new GetSketchQuery(sketchId);
                    var result = await sender.Send(query, cancellation);

                    if (result.IsFailed)
                        return TypedResults.NotFound();

                    return TypedResults.Ok(result.Value);
                })
                .ProducesProblem((int)HttpStatusCode.InternalServerError)
                .WithOpenApi(options => new(options)
                {
                    OperationId = "GetSketch",
                    Description = "Use this method to get sketch (draft) you created previously",
                    Tags = [new OpenApiTag() { Name = "Sketches" }],
                    Summary = "Retrieves sketch of fitness geo-object",
                });
}
