using FluentResults;
using Mediator;
using GoActive.Shared.Domain.Enums;

namespace GoActive.Modules.Geo.Application.Sketch.Create;

public sealed record CreateSketchCommand : ICommand<Result<Guid>>
{
    public required string Title { get; init; }
    public required ActivityTypes ActivityTypes { get; init; }
    public required SketchLocation Location { get; init; }
    public double? Altitude { get; init; }

    public sealed record SketchLocation
    {
        public required double Latitude { get; init; }
        public required double Longitude { get; init; }
    }
}
