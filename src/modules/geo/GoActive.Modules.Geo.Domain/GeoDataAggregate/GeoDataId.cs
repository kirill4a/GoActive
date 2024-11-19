namespace GoActive.Modules.Geo.Domain.GeoDataAggregate;

// TODO: consider to use 'ValueOf' NugetPackage (https://www.nuget.org/packages/ValueOf)
public readonly record struct GeoDataId(Guid Value)
{
    // TODO: allow initialize from this factory method only
    public static GeoDataId FromValue(Guid value)
    {
        return value == Guid.Empty ? throw new ArgumentException("Value should be non-empty Guid", nameof(value))
            : new(value);
    }
};
