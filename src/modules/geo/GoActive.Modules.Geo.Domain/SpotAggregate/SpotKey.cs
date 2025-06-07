using GoActive.Modules.Geo.Domain.ValueObjects;

namespace GoActive.Modules.Geo.Domain.SpotAggregate;

public readonly record struct SpotKey(NormalizedTitle NormalizedTitle, GeoCoordinate LocationPoint);
