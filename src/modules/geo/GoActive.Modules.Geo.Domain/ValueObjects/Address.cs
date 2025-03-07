using System.Text;

namespace GoActive.Modules.Geo.Domain.ValueObjects;

public sealed record Address
{
    private const string Separator = ", ";
    private const char Whitespace = ' ';

    private const StringComparison IgnoreCase = StringComparison.OrdinalIgnoreCase;

    private Address(string country) => Country = country;

    // TODO: Replace with value object Country(string alpha2Code, string alpha3Code, int numeric3Code, string name)
    public string Country { get; }
    public string? Region { get; private init; }
    public string? Settlement { get; private init; }
    public string? Street { get; private init; }

    public static Address Create(string country, string? region = null, string? settlement = null, string? street = null)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(country);

        ValidateAddressPart(country, nameof(country));
        ValidateAddressPart(region, nameof(region));
        ValidateAddressPart(settlement, nameof(settlement));
        ValidateAddressPart(street, nameof(street));

        if (!string.IsNullOrWhiteSpace(street) && string.IsNullOrWhiteSpace(region) && string.IsNullOrWhiteSpace(settlement))
        {
            throw new ArgumentException(
                $"In case of {nameof(street)} specified - {nameof(region)} or {nameof(settlement)} should be specified too",
                nameof(street));
        }

        return new(country)
        {
            Region = region,
            Settlement = settlement,
            Street = street,
        };
    }

    public bool Contains(string text) =>
        !string.IsNullOrWhiteSpace(text) &&
        (Country.Contains(text, IgnoreCase) ||
         (Region?.Contains(text, IgnoreCase) ?? false) ||
         (Settlement?.Contains(text, IgnoreCase) ?? false) ||
         (Street?.Contains(text, IgnoreCase) ?? false));

    private static void ValidateAddressPart(string? part, string paramName)
    {
        if (string.IsNullOrWhiteSpace(part))
        {
            return;
        }

        if (part.StartsWith(Whitespace) || part.EndsWith(Whitespace))
            throw new ArgumentException($"Address {paramName} shouldn't contains heading and trailing white spaces.");
    }

    public override string ToString()
    {
        var builder = new StringBuilder();
        var values = new string?[] { Street, Settlement, Region, Country }.Where(x => !string.IsNullOrWhiteSpace(x));
        builder.AppendJoin(Separator, values);

        return builder.ToString();
    }
}
