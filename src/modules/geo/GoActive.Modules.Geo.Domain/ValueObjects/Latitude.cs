namespace GoActive.Modules.Geo.Domain.ValueObjects;

public readonly record struct Latitude
{
    public Latitude(double latitudeValue)
    {
        if (latitudeValue < -90 || latitudeValue > 90)
            throw new ArgumentOutOfRangeException(nameof(latitudeValue),
                                                  latitudeValue,
                                                  "Latitude should be in the range of -90 - +90 inclusive");
        Value = latitudeValue;
    }

    public double Value { get; }
}
