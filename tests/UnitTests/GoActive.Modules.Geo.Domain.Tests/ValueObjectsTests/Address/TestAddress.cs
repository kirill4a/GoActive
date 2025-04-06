using FluentAssertions;

using GoActive.Modules.Geo.Domain.ValueObjects;

namespace GoActive.Modules.Geo.Domain.Tests.ValueObjectsTests;

public class TestAddress
{
    [Theory]
    [ClassData(typeof(AddressWrongTestData))]
    public void Create_FromWrongValue_ShouldThrowException(string country, string? region, string? settlement, string? street)
    {
        // Act
        var action = () => Address.Create(country, region, settlement, street);

        // Assert
        action.Should().Throw<ArgumentException>();
    }

    [Theory]
    [ClassData(typeof(AddressValidTestData))]
    public void Create_FromValidValue_ShouldCreated(string country, string region, string settlement, string street, string building, string postCode)
    {
        // Act
        var address = Address.Create(country, region, settlement, street, building, postCode);

        // Assert
        address.Should().NotBeNull();
        address.Country.Should().Be(country);
        address.Region.Should().Be(region);
        address.Settlement.Should().Be(settlement);
        address.Street.Should().Be(street);
        address.Building.Should().Be(building);
        address.PostCode.Should().Be(postCode);
    }

    [Fact]
    public void Create_StreetOnly_ShouldThrowException()
    {
        // Act
        var action = () => Address.Create("AQ", null, null, "Any street");

        // Assert
        action.Should().Throw<ArgumentException>().WithParameterName(nameof(Address.Street).ToLower());
    }

    [Theory]
    [InlineData("Romania", "Transilvania", "Cluj-Napoca", "Strada Transilvaniei", "qwErtY")]
    [InlineData("Romania", null, "Cluj-Napoca", "Strada Transilvaniei", "qwErtY")]
    [InlineData("Romania", "Transilvania", null, "Strada Transilvaniei", "qwErtY")]
    [InlineData("Romania", "Transilvania", "Cluj-Napoca", null, "qwErtY")]
    [InlineData("Romania", "Transilvania", "Cluj-Napoca", "Strada Transilvaniei", null)]
    public void Contains_WhenNotContainsText_ShouldBeFalse(string country,
                                                           string? region,
                                                           string? settlement,
                                                           string? street,
                                                           string? textToSearch)
    {
        // Arrange
        var address = Address.Create(country, region, settlement, street);

        // Act
        var contains = address.Contains(textToSearch!);

        // Assert
        contains.Should().BeFalse();
    }

    [Theory]
    [InlineData("Romania", "Transilvania", "Cluj-Napoca", "Strada Transilvaniei", "omA")]
    [InlineData("Romania", "Transilvania", "Cluj-Napoca", "Strada Transilvaniei", "vaNia")]
    [InlineData("Romania", "Transilvania", "Cluj-Napoca", "Strada Transilvaniei", "POC")]
    [InlineData("Romania", "Transilvania", "Cluj-Napoca", "Strada Transilvaniei", "rad")]
    [InlineData("Romania", null, "Cluj-Napoca", "Strada Transilvaniei", "rad")]
    [InlineData("Romania", "Transilvania", null, "Strada Transilvaniei", "siLV")]
    [InlineData("Romania", "Transilvania", "Cluj-Napoca", null, "POC")]
    public void Contains_WhenContainsText_ShouldBeTrue(string country,
                                                       string? region,
                                                       string? settlement,
                                                       string? street,
                                                       string textToSearch)
    {
        // Arrange
        var address = Address.Create(country, region, settlement, street);

        // Act
        var contains = address.Contains(textToSearch);

        // Assert
        contains.Should().BeTrue();
    }

    [Theory]
    [InlineData(null, null, null, null, null, "SI", "SI")]
    [InlineData(null, null, null, "Upper Carniola", "4283", "SI", "Upper Carniola, 4283, SI")]
    [InlineData(null, null, "Bled", null, null, "SI", "Bled, SI")]
    [InlineData(null, null, "Bled", "Upper Carniola", null, "SI", "Bled, Upper Carniola, SI")]
    [InlineData("15a", "Cesta Svobode", "Bled", null, null, "SI", "15a, Cesta Svobode, Bled, SI")]
    [InlineData("15a", "Cesta Svobode", "Bled", "Upper Carniola", "4283", "SI", "15a, Cesta Svobode, Bled, Upper Carniola, 4283, SI")]
    public void ToString_ShouldBeStringWithSeparator(string? building,
                                                     string? street,
                                                     string? settlement,
                                                     string? region,
                                                     string? postCode,
                                                     string country,
                                                     string? expectedText)
    {
        // Arrange
        var address = Address.Create(country, region, settlement, street, building, postCode);

        // Act
        var actualText = address.ToString();

        // Assert
        actualText.Should().Be(expectedText);
    }
}
