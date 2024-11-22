using FluentResults;
using MediatR;
using GoActive.Shared.Domain.Enums;

namespace GoActive.Modules.Geo.Application.Sketch.Get;

internal class GetSketchQueryHandler : IRequestHandler<GetSketchQuery, Result<SketchDto>>
{
    public Task<Result<SketchDto>> Handle(GetSketchQuery query, CancellationToken cancellation)
    {
        var random = new Random();

        double latitude = random.Next(-90, 90) + random.NextDouble();
        double longitude = random.Next(-180, 180) + random.NextDouble();

        var result = Result.Ok(new SketchDto(query.SketchId,
                                             "Some title",
                                             new(latitude, longitude),
                                             [ActivityTypes.NordicSki]));
        return Task.FromResult(result);
    }
}
