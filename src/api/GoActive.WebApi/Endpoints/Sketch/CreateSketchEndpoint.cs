using System.Net;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.OpenApi.Models;
using Mediator;
using GoActive.WebApi.Endpoints.Sketch.Requests;
using GoActive.WebApi.Infrastructure.Endpoints;
using GoActive.WebApi.Infrastructure.Filters;
using GoActive.Modules.Geo.Application.Sketch.Create;
using GoActive.WebApi.Infrastructure;

using SketchDto = GoActive.Modules.Geo.Application.Sketch.SketchDto;

namespace GoActive.WebApi.Endpoints.Sketch;

internal class CreateSketchEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("sketches/",
                    async Task<Results<ProblemHttpResult, CreatedAtRoute<SketchDto>>> (CreateSketchRequest request,
                                                                                  ISender sender,
                                                                                  CancellationToken cancellation) =>
                    {
                        var command = new CreateSketchCommand
                        {
                            ActivityTypes = request.ActivityTypes.Aggregate((x, y) => x | y),
                            Title = request.Title,
                            Location = new()
                            {
                                Latitude = request.Location.Latitude,
                                Longitude = request.Location.Longitude,
                            },
                            Altitude = request.Altitude,
                        };

                        var result = await sender.Send(command, cancellation);

                        if (result.IsFailed)
                        {
                            var problemDetails = $"unexpected errors: {string.Join("; ", result.Errors)}";
                            return TypedResults.Problem(detail: problemDetails);
                        }

                        return TypedResults.CreatedAtRoute(result.Value,
                                                           routeName: Constants.Routes.GetSketch,
                                                           routeValues: new { sketchId = result.Value.Id });
                    })
            .WithRequestValidation<CreateSketchRequest>()
            .ProducesProblem((int)HttpStatusCode.InternalServerError)
            .WithOpenApi(options => new(options)
            {
                OperationId = "CreateSketch",
                Description = "Use this method to create sketch (draft) you can customise later",
                Tags = [new OpenApiTag() { Name = "Sketches" }],
                Summary = "Creates sketch of fitness geo-object",
            });
}
