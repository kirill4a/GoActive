using FluentResults;
using Mediator;
using GoActive.Shared.Domain.Enums;

namespace GoActive.Modules.Geo.Application.Sketch.Get;

public sealed class GetSketchQueryHandler : IQueryHandler<GetSketchQuery, Result<SketchDto>>
{
    public ValueTask<Result<SketchDto>> Handle(GetSketchQuery query, CancellationToken cancellation)
    {
        var random = new Random();

        var latitude = random.Next(-90, 90) + random.NextDouble();
        var longitude = random.Next(-180, 180) + random.NextDouble();

        var result = Result.Ok(new SketchDto(query.SketchId,
                                             "Some title",
                                             new(latitude, longitude),
                                             [ActivityType.NordicSki]));
        return ValueTask.FromResult(result);
    }
}
