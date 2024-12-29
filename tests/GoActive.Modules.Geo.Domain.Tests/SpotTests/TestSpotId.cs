using FluentAssertions;

using GoActive.Modules.Geo.Domain.SpotAggregate;

namespace GoActive.Modules.Geo.Domain.Tests.SpotTests;

public class TestSpotId
{
    public static readonly TheoryData<Guid> WrongArguments = new() { Guid.Empty, new Guid(new byte[16]) };

    [Theory]
    [MemberData(nameof(WrongArguments))]
    public void Create_FromWrongValue_ShouldThrowException(Guid id)
    {
        // Act
        var function = () => SpotId.FromValue(id);

        // Assert
        function.Should().ThrowExactly<ArgumentException>().WithMessage("Value should be non-empty Guid*");
    }

    [Fact]
    public void Create_FromValidValue_ShouldCreated()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var spotId = SpotId.FromValue(id);

        // Assert
        spotId.Value.Should().Be(id);
    }

    [Fact]
    public void WhenDefault_ShouldBeEmpty()
    {
        // Act
        SpotId id = default;

        // Assert
        id.Should().Be(default(SpotId));
        id.Value.Should().Be(Guid.Empty);
    }
}
