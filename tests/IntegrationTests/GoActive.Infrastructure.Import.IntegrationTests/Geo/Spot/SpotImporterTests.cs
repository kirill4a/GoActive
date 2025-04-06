using System.Configuration;
using System.Text;

using FluentAssertions;

using GoActive.Infrastructure.Import.Geo;
using GoActive.Infrastructure.Import.Geo.Models;
using GoActive.Infrastructure.Import.Geo.Spot;
using GoActive.Infrastructure.Import.IntegrationTests.Configuration;
using GoActive.Infrastructure.Storage.Geo;
using GoActive.Infrastructure.Storage.Geo.DI;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;

using Testcontainers.PostgreSql;

namespace GoActive.Infrastructure.Import.IntegrationTests.Geo.Spot;

public sealed class SpotImporterTests : IAsyncLifetime
{
    private const string ConfigurationSectionName = "TestContainers";
    private readonly Mock<ILogger<SpotImporter>> _loggerkMock = new();
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
                .AddScoped(_ => _loggerkMock.Object)
                .AddScoped<IGeoJsonLinesImporter, SpotImporter>()
                .BuildServiceProvider();

        await using var scope = CreateAsyncScope();
        await CreateDatabase(scope.ServiceProvider);
    }

    public Task DisposeAsync() => _postGisContainer.DisposeAsync().AsTask();

    [Theory]
    [MemberData(nameof(CorrectData))]
    public async Task ImportSpots_WhenCorrectSource_ShouldImportDoneAndSaveData(string source, ImportResult expectedResult)
    {
        // Arrange
        var importer = _serviceProvider.GetRequiredService<IGeoJsonLinesImporter>();
        using var stream = MakeStream(source);

        // Act
        var result = await importer.Import(stream, CancellationToken.None);

        // Assert
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

    private static MemoryStream MakeStream(string content)
    {
        var bytes = Encoding.UTF8.GetBytes(content);
        return new MemoryStream(bytes);
    }

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

    private AsyncServiceScope CreateAsyncScope() => _serviceProvider.CreateAsyncScope();
}