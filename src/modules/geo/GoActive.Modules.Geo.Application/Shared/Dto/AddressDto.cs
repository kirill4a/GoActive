namespace GoActive.Modules.Geo.Application.Shared.Dto;

/// <summary>
/// Represents the address of some object.
/// </summary>
public sealed record AddressDto
{
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

    /// <summary>
    /// Building number
    /// </summary>
    public string? Building { get; init; }
}
