namespace GoActive.Modules.Geo.Domain.SketchAggregate;

// TODO: consider to use 'ValueOf' NugetPackage (https://www.nuget.org/packages/ValueOf)
public readonly record struct SketchId
{
    private SketchId(Guid value)
    {
        Value = value;
    }

    public static SketchId FromValue(Guid value)
    {
        if (value == Guid.Empty)
            throw new ArgumentException("Value should be non-empty Guid", nameof(value));

        return new(value);
    }

    public Guid Value { get; }
}