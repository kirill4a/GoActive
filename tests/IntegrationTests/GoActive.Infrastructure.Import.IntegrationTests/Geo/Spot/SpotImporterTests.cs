using System.Configuration;

using FluentAssertions;

using GoActive.Infrastructure.Import.Geo;
using GoActive.Infrastructure.Import.Geo.Extensions;
using GoActive.Infrastructure.Import.Geo.Models;
using GoActive.Infrastructure.Import.Geo.Spot;
using GoActive.Infrastructure.Import.IntegrationTests.Configuration;
using GoActive.Infrastructure.Storage.Geo;
using GoActive.Infrastructure.Storage.Geo.DI;
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
            
            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5,50.5]},"properties":{"title":"Spot 2","activities":["Workout"]}}
            {"type":"Feature","geometry":{"type":"Point","coordinates":[10.5,10.5]},"properties":{"title":"Spot 1","activities":["NordicSki"]}}

            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5,50.5]},"properties":{"title":"Spot 3","activities":["NordicSki","Workout"]}}
                
            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5,50.5]},"properties":{"title":"Spot 4","activities":["NordicSki","Workout"]}}

            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5,50.5]},"properties":{"title":"Spot 5","activities":["NordicSki","Workout"]}}
            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5,50.5]},"properties":{"title":"Spot 6","activities":["NordicSki","Workout"]}}
            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5,50.5]},"properties":{"title":"Spot 7","activities":["NordicSki","Workout"]}}
                        
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