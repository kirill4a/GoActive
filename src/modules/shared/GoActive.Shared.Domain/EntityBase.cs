using MediatR;

namespace GoActive.Shared.Domain;

// <summary>
/// Base domain entity
/// </summary>
public abstract class EntityBase<TId> where TId : IEquatable<TId>
{
    private readonly List<INotification> _domainEvents = [];

    protected EntityBase(TId id)
    {
        ArgumentNullException.ThrowIfNull(id, nameof(id));
        Id = id;
    }

    public TId Id { get; }

    public required DateTime CreatedAt { get; init; }

    public DateTime? UpdatedAt { get; init; }

    public static IEqualityComparer<EntityBase<TId>> IdEqualityComparer
        =>
        EqualityComparer<EntityBase<TId>>.Create((left, right)
            =>
            left is null
                ? right is null
                : right is not null
                  && left.GetType() == right.GetType()
                  && left.Id.Equals(right.Id));

    public IReadOnlyCollection<INotification> DomainEvents => _domainEvents.AsReadOnly();

    public void AddDomainEvent(INotification domainEvent) => _domainEvents.Add(domainEvent);

    public void ClearDomainEvents() => _domainEvents.Clear();
}
