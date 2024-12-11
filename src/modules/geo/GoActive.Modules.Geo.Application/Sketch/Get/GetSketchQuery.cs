using FluentResults;

using Mediator;

namespace GoActive.Modules.Geo.Application.Sketch.Get;

/// <summary>
/// Query to retrieve sketch
/// </summary>
/// <param name="SketchId">Sketch indentifier</param>
public readonly record struct GetSketchQuery(Guid SketchId) : IQuery<Result<SketchDto>>;
