using FluentAssertions;

using GoActive.Modules.Geo.Domain.ValueObjects;

namespace GoActive.Modules.Geo.Domain.Tests.ValueObjectsTests;

public class TestAddressId
{
    public static readonly TheoryData<Guid> WrongArguments = new() { Guid.Empty, new Guid(new byte[16]) };

    [Theory]
    [MemberData(nameof(WrongArguments))]
    public void Create_FromWrongValue_ShouldThrowException(Guid id)
    {
        // Act
        var function = () => AddressId.FromValue(id);

        // Assert
        function.Should().ThrowExactly<ArgumentException>().WithMessage("Value should be non-empty Guid*");
    }

    [Fact]
    public void Create_FromValidValue_ShouldCreated()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var addressId = AddressId.FromValue(id);

        // Assert
        addressId.Value.Should().Be(id);
    }

    [Fact]
    public void WhenDefault_ShouldBeEmpty()
    {
        // Act
        AddressId id = default;

        // Assert
        id.Should().Be(default(AddressId));
        id.Value.Should().Be(Guid.Empty);
    }
}
