using GoActive.Modules.Geo.Application.Shared.Dto;

namespace GoActive.WebApi.Endpoints.Workout.Models;

public record CalisthenicsParkDto(string Name, GeoLocationDto Location);