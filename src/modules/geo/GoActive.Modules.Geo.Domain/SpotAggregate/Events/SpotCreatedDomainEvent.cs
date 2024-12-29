using GoActive.Shared.Domain;

namespace GoActive.Modules.Geo.Domain.SpotAggregate.Events;

/// <summary>
/// Domain event occuring when geo spot has been created
/// </summary>
/// <param name="SpotId">Identifier of the created spot.</param>
public readonly record struct SpotCreatedDomainEvent(SpotId SpotId) : IDomainEvent;
