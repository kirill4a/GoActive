using GoActive.Shared.Domain;

namespace GoActive.Modules.Geo.Domain.SketchAggregate.Events;

/// <summary>
/// Domain event occuring when geo spot has been applied.
/// </summary>
/// <param name="Sketch">The applied sketch.</param>
public record SketchAppliedDomainEvent(Sketch Sketch) : IDomainEvent;
