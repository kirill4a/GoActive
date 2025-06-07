namespace GoActive.Modules.Geo.Domain.ValueObjects;

public record NormalizedTitle
{
    private const string Whitespace = " ";

    private NormalizedTitle(string value) => Value = value;

    public string Value { get; }

    public static NormalizedTitle FromValue(string value)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(value);

        return new(value
                    .Trim()
                    .Replace(Whitespace, string.Empty)
                    .ToLowerInvariant());
    }
}
