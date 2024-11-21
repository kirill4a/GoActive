namespace GoActive.Modules.Geo.Domain.ValueObjects;

public record Title
{
    private Title(string value) => Value = value;

    public string Value { get; }

    public static Title FromValue(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        if(value.StartsWith(" ") || value.EndsWith(" "))
            throw new ArgumentException("Title value shouldn't contains heading and trailing white spaces.");

        return value.Length switch
        {
            > 100 => throw new ArgumentException("Title value cannot be longer than 100 characters."),
            <= 1 => throw new ArgumentException("Title value must be more than one character."),
            _ => new(value),
        };
    }
}
