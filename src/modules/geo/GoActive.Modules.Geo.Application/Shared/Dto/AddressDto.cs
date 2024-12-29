using System.Text;

namespace GoActive.Modules.Geo.Application.Shared.Dto;

/// <summary>
/// Represents the address of some object.
/// </summary>
public sealed record AddressDto
{
    private const string Separator = ", ";

    /// <summary>
    /// Country
    /// </summary>
    public string? Country { get; init; }

    /// <summary>
    /// Region of the country
    /// </summary>
    public string? Region { get; init; }

    /// <summary>
    /// Settlement (city, town, village, etc)
    /// </summary>
    public string? Settlement { get; init; }

    /// <summary>
    /// Street
    /// </summary>
    public string? Street { get; init; }

    public override string ToString()
    {
        var builder = new StringBuilder();

        if (!string.IsNullOrWhiteSpace(Street))
            builder.AppendJoin(Separator, Street);

        if (!string.IsNullOrWhiteSpace(Settlement))
            builder.AppendJoin(Separator, Settlement);

        if (!string.IsNullOrWhiteSpace(Region))
            builder.AppendJoin(Separator, Region);

        if (!string.IsNullOrWhiteSpace(Country))
            builder.AppendJoin(Separator, Country);

        return builder.ToString();
    }
}
