using Microsoft.AspNetCore.Http.HttpResults;
using MediatR;
using GoActive.WebApi.Endpoints.Shared.Requests;
using GoActive.WebApi.Infrastructure.Endpoints;
using GoActive.WebApi.Infrastructure.Filters;
using GoActive.Modules.Geo.Application.Commands;
using System.Net;

namespace GoActive.WebApi.Endpoints.Shared;

internal class CreateSketchEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("sketches/",
                    async Task<Results<ProblemHttpResult, CreatedAtRoute<Guid>>> (CreateSketchRequest request,
                                                                                  ISender sender,
                                                                                  CancellationToken cancellation) =>
                    {
                        var command = new CreateSketchCommand
                        {
                            Title = request.Title,
                            Location = new()
                            {
                                Latitude = request.Location.Latitude,
                                Longitude = request.Location.Longitude
                            }
                        };

                        var result = await sender.Send(command, cancellation);

                        if (result.IsFailed)
                        {
                            var problemDetails = $"unexpected errors: {string.Join("; ", result.Errors)}";
                            return TypedResults.Problem(detail: problemDetails);
                        }
                        return TypedResults.CreatedAtRoute(result.Value);
                    })
            .WithRequestValidation<CreateSketchRequest>()
            .WithName("CreateSketch")
            .WithTags("Sketches")
            .ProducesProblem((int)HttpStatusCode.InternalServerError)
            .WithOpenApi();
}
