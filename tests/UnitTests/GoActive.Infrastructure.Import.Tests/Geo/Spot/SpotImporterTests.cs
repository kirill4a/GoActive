using FluentAssertions;

using GoActive.Infrastructure.Import.Geo;
using GoActive.Infrastructure.Import.Geo.Models;
using GoActive.Infrastructure.Import.Geo.Spot;
using GoActive.Modules.Geo.Application.Shared.Storage;
using GoActive.Modules.Geo.Application.Spot.Create;
using GoActive.Tests.Common;

using Mediator;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace GoActive.Infrastructure.Import.Tests.Geo.Spot;

public class SpotImporterTests
{
    private readonly Mock<ISpotCreator> _spotCreatorMock = new();
    private readonly Mock<IUnitOfWork> _unitOfWorkMock = new();
    private readonly Mock<ILogger<SpotImporter>> _loggerMock = new();
    private readonly IServiceProvider _serviceProvider;
    private readonly IGeoJsonLinesImporter<BatchImportOptions> _subject;

    public SpotImporterTests()
    {
        _serviceProvider = new ServiceCollection()
            .AddMediator(options => options.ServiceLifetime = ServiceLifetime.Scoped)
            .AddScoped<ISpotCreator>(_ => _spotCreatorMock.Object)
            .AddScoped<IUnitOfWork>(_ => _unitOfWorkMock.Object)
            .BuildServiceProvider();

        var sender = _serviceProvider.GetRequiredService<ISender>();
        _subject = new SpotImporter(sender, _loggerMock.Object);
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
    };

    public static TheoryData<string, ImportResult> CorrectData => new()
    {
        {
            """
            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5,50.5]},"properties":{"title":"Spot 1","activities":["NordicSki","Workout"]}}
            """,
            new ImportResult(Total: 1, Successes: 1, Errors: 0)
        },
        {
            """
            
            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5,50.5]},"properties":{"title":"Spot 2","activities":["Workout"]}}
            {"type":"Feature","geometry":{"type":"Point","coordinates":[10.5,10.5]},"properties":{"title":"Spot 1","activities":["NordicSki"]}}

            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5,50.5]},"properties":{"title":"Spot 3","activities":["NordicSki","Workout"]}}
                
            
            """,
            new ImportResult(Total: 3, Successes: 3, Errors: 0)
        },
    };

    [Theory]
    [MemberData(nameof(IncorrectData))]
    public async Task ImportSpots_WhenIncorrectSource_ShouldImportNone(string source, ImportResult expectedResult)
    {
        // Arrange
        _unitOfWorkMock.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(0);
        using var stream = source.AsMemoryStream();

        // Act
        var result = await _subject.Import(stream, default, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(expectedResult);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Never);
    }

    [Theory]
    [MemberData(nameof(CorrectData))]
    public async Task ImportSpots_WhenCorrectSource_ShouldSingleImportDone(string source, ImportResult expectedResult)
    {
        // Arrange
        _unitOfWorkMock.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(expectedResult.Successes);
        using var stream = source.AsMemoryStream();

        // Act
        var result = await _subject.Import(stream, default, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(expectedResult);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task ImportSpots_WhenCorrectSource_ShouldBatchImportDone()
    {
        // Arrange
        const string source = """
            
            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5,50.5]},"properties":{"title":"Spot 2","activities":["Workout"]}}
            {"type":"Feature","geometry":{"type":"Point","coordinates":[10.5,10.5]},"properties":{"title":"Spot 1","activities":["NordicSki"]}}

            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5,50.5]},"properties":{"title":"Spot 3","activities":["NordicSki","Workout"]}}
                
            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5,50.5]},"properties":{"title":"Spot 4","activities":["NordicSki","Workout"]}}

            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5,50.5]},"properties":{"title":"Spot 5","activities":["NordicSki","Workout"]}}
            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5,50.5]},"properties":{"title":"Spot 6","activities":["NordicSki","Workout"]}}
            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5,50.5]},"properties":{"title":"Spot 7","activities":["NordicSki","Workout"]}}
            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5,50.5]},"properties":{"title":"Spot 8","activities":["NordicSki","Workout"]}}
                        
            """;
        ushort batchSize = 4;
        var options = new BatchImportOptions { BatchSize = batchSize };
        var expectedResult = new ImportResult(Total: 8, Successes: 8, Errors: 0);

        _unitOfWorkMock.Setup(x => x.CommitAsync(It.IsAny<CancellationToken>())).ReturnsAsync(batchSize);
        using var stream = source.AsMemoryStream();

        // Act
        var result = await _subject.Import(stream, options, CancellationToken.None);

        // Assert
        // TODO: assert batches (events) counts
        result.Should().NotBeNull();
        result.Should().Be(expectedResult);
        _unitOfWorkMock.Verify(x => x.CommitAsync(It.IsAny<CancellationToken>()), Times.Exactly(2));
    }
}
