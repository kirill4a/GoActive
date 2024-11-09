using GoActive.WebApi.Endpoints.Shared.Models;

namespace GoActive.WebApi.Endpoints.NordicSki.Models;

public record SkiTrailDto(string Name, GeoLocationDto MeanLocation, IEnumerable<SkiTrailPathDto>? Paths);
