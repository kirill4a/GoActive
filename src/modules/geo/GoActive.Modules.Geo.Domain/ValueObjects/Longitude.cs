namespace GoActive.Modules.Geo.Domain.ValueObjects;

public readonly record struct Longitude
{
    public Longitude(double longitudeValue)
    {
        if (longitudeValue < -180 || longitudeValue > 180)
            throw new ArgumentOutOfRangeException(nameof(longitudeValue),
                                                  longitudeValue,
                                                  "Longitude should be in the range of -180 - +180 inclusive");
        Value = longitudeValue;

    }
    public double Value { get; }
}
