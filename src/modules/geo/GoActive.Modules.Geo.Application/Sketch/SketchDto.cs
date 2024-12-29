using GoActive.Modules.Geo.Application.Shared.Dto;
using GoActive.Shared.Domain.Enums;

namespace GoActive.Modules.Geo.Application.Sketch;

/// <summary>
/// The sketch of sport activity object.
/// </summary>
/// <param name="Id">Sketch identidier</param>
/// <param name="Title">Sketch title</param>
/// <param name="Location">Sketch location</param>
/// <param name="ActivityTypes">Activities supported by the sketch</param>
public sealed record SketchDto(Guid Id, string Title, GeoLocationDto Location, IEnumerable<ActivityTypes> ActivityTypes);
