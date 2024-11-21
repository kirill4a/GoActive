using FluentResults;
using MediatR;
using GoActive.Shared.Domain.Enums;

namespace GoActive.Modules.Geo.Application.Sketch.Get;

internal class GetSketchQueryHandler : IRequestHandler<GetSketchQuery, Result<SketchDto>>
{
    public Task<Result<SketchDto>> Handle(GetSketchQuery query, CancellationToken cancellation)
    {
        var latitudeRandom = new Random();
        var longitudeRandom = new Random();

        var result = Result.Ok(new SketchDto(Guid.NewGuid(),
                                             "Some title",
                                             new(latitudeRandom.Next(-90, 90), longitudeRandom.Next(-180, 180)),
                                             [ActivityTypes.NordicSki]));
        return Task.FromResult(result);
    }
}
