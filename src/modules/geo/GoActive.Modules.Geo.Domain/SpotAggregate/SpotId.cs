namespace GoActive.Modules.Geo.Domain.SpotAggregate;

public readonly record struct SpotId
{
    private SpotId(Guid value)
    {
        Value = value;
    }

    public Guid Value { get; }

    public static SpotId FromValue(Guid value)
    {
        return value == Guid.Empty ? throw new ArgumentException("Value should be non-empty Guid", nameof(value)) : new(value);
    }
}
