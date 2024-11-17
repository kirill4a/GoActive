using GoActive.Shared.Domain;

namespace GoActive.Modules.Geo.Domain.SketchAggregate.Events;

/// <summary>
/// Domain event occuring when sketch has been created
/// </summary>
/// <param name="Sketch"></param>
public record SketchCreatedDomainEvent(SketchId SketchId) : IDomainEvent;
