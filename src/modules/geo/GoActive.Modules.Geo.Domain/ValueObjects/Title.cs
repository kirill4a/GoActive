namespace GoActive.Modules.Geo.Domain.ValueObjects;

public record Title
{
    private Title(string value) => Value = value;

    public string Value { get; }

    public static Title FromValue(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);
        return new(value);
    }
}
