using FluentAssertions;

using GoActive.Modules.Geo.Domain.SketchAggregate;

namespace GoActive.Modules.Geo.Domain.Tests.SketchTests;

public class TestSketchId
{
    public static readonly TheoryData<Guid> WrongArguments = new() { Guid.Empty, new Guid(new byte[16]) };

    [Theory]
    [MemberData(nameof(WrongArguments))]
    public void Create_FromWrongValue_ShouldThrowException(Guid id)
    {
        // Act
        var function = () => SketchId.FromValue(id);

        // Assert
        function.Should().ThrowExactly<ArgumentException>().WithMessage("Value should be non-empty Guid*");
    }

    [Fact]
    public void Create_FromValidValue_ShouldCreated()
    {
        // Arrange
        var id = Guid.NewGuid();

        // Act
        var sketchId = SketchId.FromValue(id);

        // Assert
        sketchId.Value.Should().Be(id);
    }

    [Fact]
    public void WhenDefault_ShouldBeEmpty()
    {
        // Act
        SketchId id = default;

        // Assert
        id.Should().Be(default(SketchId));
        id.Value.Should().Be(Guid.Empty);
    }
}