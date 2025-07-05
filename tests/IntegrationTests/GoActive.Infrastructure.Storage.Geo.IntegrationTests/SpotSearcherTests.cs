using System.Configuration;

using FluentAssertions;

using GoActive.Infrastructure.Storage.Geo.DI;
using GoActive.Modules.Geo.Application.Spot.Create;
using GoActive.Modules.Geo.Application.Spot.Search;
using GoActive.Modules.Geo.Domain.SpotAggregate;
using GoActive.Modules.Geo.Domain.ValueObjects;
using GoActive.Shared.Domain.Enums;
using GoActive.Tests.Common.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Testcontainers.PostgreSql;

using DomainSpot = GoActive.Modules.Geo.Domain.SpotAggregate.Spot;

namespace GoActive.Infrastructure.Storage.Geo.IntegrationTests;

[Trait("Category", "Integration")]
[Trait("Category", "Geo")]
public sealed class SpotSearcherTests : IAsyncLifetime
{
    private const string ConfigurationSectionName = "TestContainers";
    private readonly PostgreSqlContainer _postGisContainer;
    private IServiceProvider _serviceProvider = default!;

    public SpotSearcherTests()
    {
        var configuration = new ConfigurationBuilder()
            .AddUserSecrets<SpotSearcherTests>()
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

    public async Task InitializeAsync()
    {
        await _postGisContainer.StartAsync();

        var connectionString = _postGisContainer.GetConnectionString();
        var migrationsAssembly = typeof(Storage.Geo.Migrations.Program).Assembly;

        _serviceProvider = new ServiceCollection()
                .AddGeoStorage(connectionString, migrationsAssembly)
                .BuildServiceProvider();

        await using var scope = CreateAsyncScope();
        await CreateDatabase(scope.ServiceProvider);
        await SeedDatabase(scope.ServiceProvider);
    }

    public Task DisposeAsync() => _postGisContainer.DisposeAsync().AsTask();

    [Fact]
    public async Task SearchSpots_WhenTitleMatches_ShouldReturnData()
    {
        // Arrange
        const string searchQuery = "eSt";
        var searcher = _serviceProvider.GetRequiredService<ISpotSearcher>();

        // Act
        var result = await searcher.SearchBySpot(searchQuery, [], CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        result.Should().AllSatisfy(x => x.Title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase));
    }

    [Fact]
    public async Task SearchSpots_WhenTitleAndActivityMatches_ShouldReturnData()
    {
        // Arrange
        const string searchQuery = "eSt";
        var activity = ActivityType.Workout;
        var searcher = _serviceProvider.GetRequiredService<ISpotSearcher>();

        // Act
        var result = await searcher.SearchBySpot(searchQuery, [activity], CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        result.Should().AllSatisfy(x =>
        {
            x.Title.Contains(searchQuery, StringComparison.OrdinalIgnoreCase).Should().BeTrue();
            x.Activities.Contains(activity).Should().BeTrue();
        });
    }

    private static async Task CreateDatabase(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<GeoContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();
    }

    private static async Task SeedDatabase(IServiceProvider serviceProvider)
    {
        var spot = DomainSpot.Create(
            id: SpotId.FromValue(Guid.NewGuid()),
            title: Title.FromValue("Test Spot"),
            locationPoint: GeoCoordinate.FromLocation(GeoLocation.FromLatLon(52.616670, 39.600000)),
            activities: [ActivityType.Workout],
            description: "A test spot for integration tests");

        var creator = serviceProvider.GetRequiredService<ISpotCreator>();
        await creator.BulkInsertSpotsAsync([spot], CancellationToken.None);
    }

    private AsyncServiceScope CreateAsyncScope() => _serviceProvider.CreateAsyncScope();
}