using FluentAssertions;

using GoActive.Infrastructure.Import.Geo;
using GoActive.Infrastructure.Import.Geo.Models;
using GoActive.Infrastructure.Import.Geo.Spot;
using GoActive.Modules.Geo.Application.Spot.Create;
using GoActive.Tests.Common;

using Microsoft.Extensions.Logging;

using DomainSpot = GoActive.Modules.Geo.Domain.SpotAggregate.Spot;

namespace GoActive.Infrastructure.Import.Tests.Geo.Spot;

public class SpotImporterTests
{
    private readonly Mock<ISpotCreator> _spotCreatorMock = new();
    private readonly Mock<ILogger<SpotImporter>> _loggerMock = new();
    private readonly IGeoJsonLinesImporter<BatchImportOptions> _subject;

    public SpotImporterTests()
    {
        _subject = new SpotImporter(_spotCreatorMock.Object, _loggerMock.Object);
    }

    public static TheoryData<string, ImportResult> IncorrectData => new()
    {
        { "", new() },
        { string.Empty, new() },
        { "QWERTY", new(Total: 0, Successes: 0, Errors: 1) },
        {
            """

                
            { "type":"Unknown","geometry":{"type":"Point","coordinates":[30.5,50.5]},"properties":{"name":"Spot 1"}
            "QWERTY"

            ASDFGH
            """,
            new ImportResult(Total: 0, Successes: 0, Errors: 3)
        },
        {
            """
            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5,50.5]},"properties":{"title":" ","activities":["NordicSki","Workout"]}}
            """,
            new ImportResult(Total: 1, Successes: 0, Errors: 1)
        },
        {
            """
            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5,50.5]},"properties":{"title":"Spot 1","activities":[""]}}
            """,
            new ImportResult(Total: 1, Successes: 0, Errors: 1)
        },
        {
            """
            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5,50.5]},"properties":{"title":"Spot 1","activities":["QWERTY"]}}
            """,
            new ImportResult(Total: 1, Successes: 0, Errors: 1)
        },
        {
            """
            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5,50.5]},"properties":{"id":"","title":"Spot 1","activities":["NordicSki","Workout"]}}
            """,
            new ImportResult(Total: 1, Successes: 0, Errors: 1)
        },
        {
            """
            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5,50.5]},"properties":{"id":"  ","title":"Spot 1","activities":["NordicSki","Workout"]}}
            """,
            new ImportResult(Total: 1, Successes: 0, Errors: 1)
        },
        {
            """
            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5,50.5]},"properties":{"id":"1","title":"Spot 1","activities":["NordicSki","Workout"]}}
            """,
            new ImportResult(Total: 1, Successes: 0, Errors: 1)
        },
    };

    public static TheoryData<string, ImportResult> CorrectData => new()
    {
        {
            """
            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5,50.5]},"properties":{"id":"070307e1-6420-4b31-af9d-9066a72d1291","title":"Spot 1","activities":["NordicSki","Workout"]}}
            """,
            new ImportResult(Total: 1, Successes: 1, Errors: 0)
        },
        {
            """
            
            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5,50.5]},"properties":{"id":"070307e1-6420-4b31-af9d-9066a72d1291","title":"Spot 2","activities":["Workout"]}}
            {"type":"Feature","geometry":{"type":"Point","coordinates":[10.5,10.5]},"properties":{"id":"070307e1-6420-4b31-af9d-9066a72d1291","title":"Spot 1","activities":["NordicSki"]}}

            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5,50.5]},"properties":{"id":"070307e1-6420-4b31-af9d-9066a72d1291","title":"Spot 3","activities":["NordicSki","Workout"]}}
                
            
            """,
            new ImportResult(Total: 3, Successes: 3, Errors: 0)
        },
    };

    [Theory]
    [MemberData(nameof(IncorrectData))]
    public async Task ImportSpots_WhenIncorrectSource_ShouldImportNone(string source, ImportResult expectedResult)
    {
        // Arrange
        _spotCreatorMock
            .Setup(x => x.BulkInsertSpotsAsync(It.IsAny<IReadOnlyCollection<DomainSpot>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(0);
        using var stream = source.AsMemoryStream();

        // Act
        var result = await _subject.Import(stream, default, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(expectedResult);
        _spotCreatorMock.Verify(
            x => x.BulkInsertSpotsAsync(It.IsAny<IReadOnlyCollection<DomainSpot>>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Theory]
    [MemberData(nameof(CorrectData))]
    public async Task ImportSpots_WhenCorrectSource_ShouldSingleImportDone(string source, ImportResult expectedResult)
    {
        // Arrange
        _spotCreatorMock
            .Setup(x => x.BulkInsertSpotsAsync(It.IsAny<IReadOnlyCollection<DomainSpot>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResult.Successes);
        using var stream = source.AsMemoryStream();

        // Act
        var result = await _subject.Import(stream, default, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(expectedResult);
        _spotCreatorMock.Verify(
            x => x.BulkInsertSpotsAsync(It.IsAny<IReadOnlyCollection<DomainSpot>>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task ImportSpots_WhenCorrectSource_ShouldBatchImportDone()
    {
        // Arrange
        const string source = """
            
            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5,50.5]},"properties":{"id":"070307e1-6420-4b31-af9d-9066a72d1291","title":"Spot 2","activities":["Workout"]}}
            {"type":"Feature","geometry":{"type":"Point","coordinates":[10.5,10.5]},"properties":{"id":"070307e1-6420-4b31-af9d-9066a72d1291","title":"Spot 1","activities":["NordicSki"]}}

            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5,50.5]},"properties":{"id":"070307e1-6420-4b31-af9d-9066a72d1291","title":"Spot 3","activities":["NordicSki","Workout"]}}
                
            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5,50.5]},"properties":{"id":"070307e1-6420-4b31-af9d-9066a72d1291","title":"Spot 4","activities":["NordicSki","Workout"]}}

            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5,50.5]},"properties":{"id":"070307e1-6420-4b31-af9d-9066a72d1291","title":"Spot 5","activities":["NordicSki","Workout"]}}
            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5,50.5]},"properties":{"id":"070307e1-6420-4b31-af9d-9066a72d1291","title":"Spot 6","activities":["NordicSki","Workout"]}}
            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5,50.5]},"properties":{"id":"070307e1-6420-4b31-af9d-9066a72d1291","title":"Spot 7","activities":["NordicSki","Workout"]}}
            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5,50.5]},"properties":{"id":"070307e1-6420-4b31-af9d-9066a72d1291","title":"Spot 8","activities":["NordicSki","Workout"]}}
                        
            """;
        ushort batchSize = 4;
        var options = new BatchImportOptions { BatchSize = batchSize };
        var expectedResult = new ImportResult(Total: 8, Successes: 8, Errors: 0);

        _spotCreatorMock
            .Setup(x => x.BulkInsertSpotsAsync(It.IsAny<IReadOnlyCollection<DomainSpot>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(batchSize);
        using var stream = source.AsMemoryStream();

        // Act
        var result = await _subject.Import(stream, options, CancellationToken.None);

        // Assert
        // TODO: assert batches (events) counts
        result.Should().NotBeNull();
        result.Should().Be(expectedResult);
        _spotCreatorMock.Verify(
            x => x.BulkInsertSpotsAsync(It.IsAny<IReadOnlyCollection<DomainSpot>>(), It.IsAny<CancellationToken>()),
            Times.Exactly(2));
    }
}
