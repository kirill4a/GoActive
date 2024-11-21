namespace GoActive.Shared.Domain;

/// <summary>
/// Base domain entity, implements equality by identifier
/// </summary>
/// <typeparam name="TId"> The type of key </typeparam>
public abstract class EntityBase<TId>
    where TId : IEquatable<TId>
{
    private readonly List<IDomainEvent> _domainEvents = [];

    protected EntityBase(TId id)
    {
        if (id?.Equals(default) ?? true)
            throw new ArgumentNullException(nameof(id));
        Id = id;
    }

    public static IEqualityComparer<EntityBase<TId>> IdEqualityComparer
    =>
    EqualityComparer<EntityBase<TId>>.Create((left, right)
        =>
        left is null
            ? right is null
            : right is not null
              && left.GetType() == right.GetType()
              && left.Id.Equals(right.Id));

    public TId Id { get; }

    public required DateTime CreatedAt { get; init; }

    public DateTime? UpdatedAt { get; init; }

    public IReadOnlyCollection<IDomainEvent> DomainEvents => _domainEvents.AsReadOnly();

    public static bool operator ==(EntityBase<TId> left, EntityBase<TId> right)
    {
        return left?.GetHashCode() == right?.GetHashCode() || IdEqualityComparer.Equals(left, right);
    }

    public static bool operator !=(EntityBase<TId> left, EntityBase<TId> right)
    {
        return !(left == right);
    }

    public override int GetHashCode() => Id.GetHashCode();

    public override bool Equals(object? obj)
    {
        return IdEqualityComparer.Equals(this, obj as EntityBase<TId>);
    }

    public void ClearDomainEvents() => _domainEvents.Clear();

    protected void AddDomainEvent(IDomainEvent domainEvent) => _domainEvents.Add(domainEvent);
}
