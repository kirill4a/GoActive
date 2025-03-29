using NetTopologySuite.Features;
using NetTopologySuite.Geometries;

namespace GoActive.Shared.Serialization.Tests.TestData;

public class FakeFeature : IFeature
{
    private readonly Dictionary<string, object> _attributes = new()
        {
            { "firstName", "John" },
            { "lastName", "Doe" },
        };

    public Geometry Geometry { get => new Point(1, 2, 3); set => throw new NotImplementedException(); }
    public Envelope BoundingBox { get => new(); set => throw new NotImplementedException(); }
    public IAttributesTable Attributes { get => new AttributesTable(_attributes); set => throw new NotImplementedException(); }

    public IFeature WithAttribute(string key, object value)
    {
        if (_attributes.ContainsKey(key))
        {
            throw new ArgumentException($"An attribute with the key '{key}' is already exists.");
        }

        _attributes.Add(key, value);
        return this;
    }
}
