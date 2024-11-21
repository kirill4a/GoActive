using GoActive.Modules.Geo.Application.Shared.Dto;

namespace GoActive.WebApi.Endpoints.NordicSki.Models;

public record SkiTrailDto(string Name, GeoLocationDto MeanLocation, IEnumerable<SkiTrailPathDto>? Paths);
