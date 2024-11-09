using MediatR;
using FluentResults;

namespace GoActive.Modules.Geo.Application.Commands;

public sealed record CreateSketchCommand : IRequest<Result<Guid>>
{
    public required string Title { get; init; }
    public required SketchLocation Location { get; init; }

    public sealed record SketchLocation
    {
        public required double Latitude { get; init; }
        public required double Longitude { get; init; }
    }
}
