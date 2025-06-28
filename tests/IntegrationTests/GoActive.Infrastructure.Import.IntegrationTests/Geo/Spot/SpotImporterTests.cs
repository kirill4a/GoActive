using System.Configuration;

using FluentAssertions;

using GoActive.Infrastructure.Import.Geo;
using GoActive.Infrastructure.Import.Geo.Extensions;
using GoActive.Infrastructure.Import.Geo.Models;
using GoActive.Infrastructure.Import.Geo.Spot;
using GoActive.Infrastructure.Import.IntegrationTests.Configuration;
using GoActive.Infrastructure.Storage.Geo;
using GoActive.Infrastructure.Storage.Geo.DI;
using GoActive.Shared.Domain.Enums;
using GoActive.Tests.Common;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;

using Testcontainers.PostgreSql;

namespace GoActive.Infrastructure.Import.IntegrationTests.Geo.Spot;

[Trait("Category", "Integration")]
[Trait("Category", "Geo")]
public sealed class SpotImporterTests : IAsyncLifetime
{
    private const string ImporterKey = nameof(SpotImporter);
    private const string ConfigurationSectionName = "TestContainers";
    private readonly Mock<ILogger<SpotImporter>> _loggerMock = new();
    private readonly PostgreSqlContainer _postGisContainer;
    private IServiceProvider _serviceProvider = default!;

    public SpotImporterTests()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<SpotImporterTests>()
            .AddEnvironmentVariables()
            .Build();

        var testsSection = configuration.GetSection(ConfigurationSectionName);
        var pgOptions = testsSection.GetSection(nameof(PgOptions)).Get<PgOptions>()
            ?? throw new ConfigurationErrorsException(
                $"{ConfigurationSectionName} section is not properly configured. Missing or incorrect '{nameof(PgOptions)}' section.");

        _postGisContainer = new PostgreSqlBuilder()
                .WithImage("postgis/postgis")
                .WithDatabase(pgOptions.Database)
                .WithUsername(pgOptions.Username)
                .WithPassword(pgOptions.Password)
                .WithCleanUp(true)
                .WithAutoRemove(true)
                .Build();
    }

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
            
            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5,50.5]},"properties":{"id":"04f25efa-426f-42ab-9410-808f1227d0ae","title":"Spot 2","activities":["Workout"]}}
            {"type":"Feature","geometry":{"type":"Point","coordinates":[10.5,10.5]},"properties":{"id":"90746885-ab62-463b-9835-b888d1d795c5","title":"Spot 1","activities":["NordicSki"]}}

            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5,50.5]},"properties":{"id":"ef01c411-0503-4be4-ae5b-a1acdca18062","title":"Spot 3","activities":["NordicSki","Workout"]}}
                
            
            """,
            new ImportResult(Total: 3, Successes: 3, Errors: 0)
        },
    };

    public async Task InitializeAsync()
    {
        await _postGisContainer.StartAsync();

        var connectionString = _postGisContainer.GetConnectionString();
        var migrationsAssembly = typeof(Storage.Geo.Migrations.Program).Assembly;

        _serviceProvider = new ServiceCollection()
                .AddGeoStorage(connectionString, migrationsAssembly)
                .AddMediator(options => options.ServiceLifetime = ServiceLifetime.Scoped)
                .AddScoped(_ => _loggerMock.Object)
                .AddSpotImport(ImporterKey)
                .BuildServiceProvider();

        await using var scope = CreateAsyncScope();
        await CreateDatabase(scope.ServiceProvider);
    }

    public Task DisposeAsync() => _postGisContainer.DisposeAsync().AsTask();

    [Theory]
    [MemberData(nameof(CorrectData))]
    public async Task ImportSpots_WhenCorrectSource_ShouldSingleImportDone(string source, ImportResult expectedResult)
    {
        // Arrange
        var importer = _serviceProvider.GetRequiredKeyedService<IGeoJsonLinesImporter<BatchImportOptions>>(ImporterKey);
        using var stream = source.AsMemoryStream();

        // Act
        var result = await importer.Import(stream, default, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(expectedResult);
        await AssertDatabase(async x =>
        {
            (await x.Spots.CountAsync()).Should().Be(expectedResult.Successes);
        });
    }

    [Fact]
    public async Task ImportSpots_WhenCorrectSource_ShouldBatchImportDone()
    {
        // Arrange
        const string source = """
            
            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5,50.5]},"properties":{"id":"070307e1-6420-4b31-af9d-9066a72d1291","title":"Spot 2","activities":["Workout"]}}
            {"type":"Feature","geometry":{"type":"Point","coordinates":[10.5,10.5]},"properties":{"id":"8b17aef8-07ab-4a6b-8543-ff8751a83c4f","title":"Spot 1","activities":["NordicSki"]}}

            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5,50.5]},"properties":{"id":"9c223f42-159a-46ac-bfa4-ecb2de0d9698","title":"Spot 3","activities":["NordicSki","Workout"]}}
                
            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5,50.5]},"properties":{"id":"0b79e0ce-68de-47fd-841f-8d6fc88807d5","title":"Spot 4","activities":["NordicSki","Workout"]}}

            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5,50.5]},"properties":{"id":"1662efbe-9ead-4c8c-80e6-07b520c200d8","title":"Spot 5","activities":["NordicSki","Workout"]}}
            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5,50.5]},"properties":{"id":"1daf4b58-edb4-409e-93de-8ce10eb1482e","title":"Spot 6","activities":["NordicSki","Workout"]}}
            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5,50.5]},"properties":{"id":"7eac018e-41ac-48fd-a8ac-e174ca7289bd","title":"Spot 7","activities":["NordicSki","Workout"]}}
                        
            """;
        ushort batchSize = 4;
        var options = new BatchImportOptions { BatchSize = batchSize };
        var expectedResult = new ImportResult(Total: 7, Successes: 7, Errors: 0);

        var importer = _serviceProvider.GetRequiredKeyedService<IGeoJsonLinesImporter<BatchImportOptions>>(ImporterKey);
        using var stream = source.AsMemoryStream();

        // Act
        var result = await importer.Import(stream, options, CancellationToken.None);

        // Assert
        // TODO: assert batches (events) counts
        result.Should().NotBeNull();
        result.Should().Be(expectedResult);
        await AssertDatabase(async x =>
        {
            (await x.Spots.CountAsync()).Should().Be(expectedResult.Successes);
            (await x.Spots.AllAsync(x => x.CreatedAt != DateTime.MinValue)).Should().BeTrue();
        });
    }

    [Fact]
    public async Task ImportSpots_WhenSourceWith2Dor3DLocations_ShouldImportDone()
    {
        // Arrange
        const string source =
            """
            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5, 50.5]},"properties":{"id":"070307e1-6420-4b31-af9d-9066a72d1291","title":"Spot A","activities":["NordicSki","Workout"]}}
            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5, 50.5, 179]},"properties":{"id":"c3b07a3e-8ecd-439f-9dcc-f162f2f8ef8b","title":"Spot B","activities":["NordicSki","Workout"]}}
            """;
        var expectedResult = new ImportResult(Total: 2, Successes: 2, Errors: 0);

        var importer = _serviceProvider.GetRequiredKeyedService<IGeoJsonLinesImporter<BatchImportOptions>>(ImporterKey);
        using var stream = source.AsMemoryStream();

        // Act
        var result = await importer.Import(stream, default, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(expectedResult);

        await AssertDatabase(async x =>
        {
            (await x.Spots.CountAsync()).Should().Be(expectedResult.Successes);
        });

        await AssertDatabase(async x =>
        {
            var spot2d = await x.Spots.OrderBy(x => x.Title).FirstAsync();
            spot2d.Should().NotBeNull();

            spot2d.Location.X.Should().Be(30.5);
            spot2d.Location.Y.Should().Be(50.5);

            spot2d.Location.Z.Should().Be(double.NaN);
            spot2d.Altitude.Should().Be(null);
        });

        await AssertDatabase(async x =>
        {
            var spot3d = await x.Spots.OrderByDescending(x => x.Title).FirstAsync();
            spot3d.Should().NotBeNull();

            spot3d.Location.X.Should().Be(30.5);
            spot3d.Location.Y.Should().Be(50.5);

            spot3d.Location.Z.Should().Be(double.NaN);
            spot3d.Altitude.Should().Be(179);
        });
    }

    [Fact]
    public async Task ImportSpots_WhenImportDone_ShouldContainConsistentData()
    {
        // Arrange
        const string source =
            """
            {"type":"Feature","geometry":{"type":"Point","coordinates":[46.236111,14.408889,1330]},"properties":{"id":"2a998154-23a4-4a85-aae3-ed15763f1259","title":"Pokljuka biathlon stadium - f","description":"Pokljuka is one of the best known Slovenian plateaus, as it holds world famous biathlon competitions. The plateau is mostly forested; the \u0160ijec peat bog is interesting for its great ecological significance and providing shelter to many animals and plants. Pokljuka is a forested high karst plateau in the Julian Alps. It is the largest closed forest area in the Triglav National Park. It is 20 km long and almost as wide. In a span from 1000 to 1400 m there are around 6300 ha of forests, in which spruce prevails.","activities":["NordicSki","Biathlon","RollerSki"]}}
            """;
        var expectedResult = new ImportResult(Total: 1, Successes: 1, Errors: 0);

        var importer = _serviceProvider.GetRequiredKeyedService<IGeoJsonLinesImporter<BatchImportOptions>>(ImporterKey);
        using var stream = source.AsMemoryStream();

        // Act
        var result = await importer.Import(stream, default, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(expectedResult);

        await AssertDatabase(async x =>
        {
            (await x.Spots.CountAsync()).Should().Be(expectedResult.Successes);

            var spot = await x.Spots.FirstAsync();
            spot.Should().NotBeNull();

            spot.Id.Should().Be(Guid.Parse("2a998154-23a4-4a85-aae3-ed15763f1259"));
            spot.Title.Should().Be("Pokljuka biathlon stadium - f");
            spot.NormalizedTitle.Should().Be("pokljukabiathlonstadium-f");
            spot.Description.Should().Be("Pokljuka is one of the best known Slovenian plateaus, as it holds world famous biathlon competitions. The plateau is mostly forested; the Šijec peat bog is interesting for its great ecological significance and providing shelter to many animals and plants. Pokljuka is a forested high karst plateau in the Julian Alps. It is the largest closed forest area in the Triglav National Park. It is 20 km long and almost as wide. In a span from 1000 to 1400 m there are around 6300 ha of forests, in which spruce prevails.");
            spot.Activities.Should().BeEquivalentTo([ActivityType.NordicSki, ActivityType.Biathlon, ActivityType.RollerSki]);

            spot.Location.X.Should().Be(46.236111);
            spot.Location.Y.Should().Be(14.408889);

            spot.Location.Z.Should().Be(double.NaN);
            spot.Altitude.Should().Be(1330);
        });
    }

    private static async Task CreateDatabase(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<GeoContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();
    }

    private AsyncServiceScope CreateAsyncScope() => _serviceProvider.CreateAsyncScope();

    private Task AssertDatabase(Func<IGeoContext, Task> action) => AssertInNewScope(sp =>
    {
        var context = sp.GetRequiredService<IGeoContext>();
        return action(context);
    });

    private async Task AssertInNewScope(Func<IServiceProvider, Task> action)
    {
        await using var scope = CreateAsyncScope();
        await action(scope.ServiceProvider);
    }
}