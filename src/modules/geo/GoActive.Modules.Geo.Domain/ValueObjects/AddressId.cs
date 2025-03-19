namespace GoActive.Modules.Geo.Domain.ValueObjects;

public readonly record struct AddressId
{
    private AddressId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static AddressId FromValue(Guid value)
    {
        return value == Guid.Empty ? throw new ArgumentException("Value should be non-empty Guid", nameof(value)) : new(value);
    }
}
