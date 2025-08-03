using System.Configuration;

using FluentAssertions;

using GoActive.Infrastructure.Storage.Geo.DI;
using GoActive.Infrastructure.Storage.Geo.Repositories;
using GoActive.Modules.Geo.Application.Address;
using GoActive.Modules.Geo.Application.Shared.Dto;
using GoActive.Modules.Geo.Application.Spot.Create;
using GoActive.Modules.Geo.Application.Spot.Search;
using GoActive.Modules.Geo.Domain.SpotAggregate;
using GoActive.Modules.Geo.Domain.ValueObjects;
using GoActive.Shared.Domain.Enums;
using GoActive.Tests.Common.Configuration;

using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Moq;

using Testcontainers.PostgreSql;

using DomainSpot = GoActive.Modules.Geo.Domain.SpotAggregate.Spot;

namespace GoActive.Infrastructure.Storage.Geo.IntegrationTests;

[Trait("Category", "Integration")]
[Trait("Category", "Geo")]
public sealed class SpotSearcherTests : IAsyncLifetime
{
    private const string CountryCode = "00";
    private const string LinkedCityName = "Test City Linked";
    private const string ConfigurationSectionName = "TestContainers";
    private readonly Guid _spotId = Guid.NewGuid();
    private readonly Guid _spotWithAddressId = Guid.NewGuid();
    private readonly Mock<ILogger<AddressRepository>> _loggerRepositoryMock = new();
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
                .AddScoped(_ => _loggerRepositoryMock.Object)
                .BuildServiceProvider();

        await using var scope = CreateAsyncScope();
        await CreateDatabase(scope.ServiceProvider);
        await SeedDatabase(scope.ServiceProvider);
    }

    public Task DisposeAsync() => _postGisContainer.DisposeAsync().AsTask();

    [Fact]
    public async Task GetSpot_WhenNotFound_ShouldReturnNull()
    {
        // Arrange
        var randomId = SpotId.FromValue(Guid.NewGuid());
        var searcher = _serviceProvider.GetRequiredService<ISpotSearcher>();

        // Act
        var result = await searcher.GetAsync(randomId, CancellationToken.None);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task GetSpot_WithNoAddress_ShouldReturnData()
    {
        // Arrange
        var expectedTitle = "Test Spot";
        var expectedLocation = new GeoLocationDto(52.616670, 39.600000);
        IEnumerable<ActivityType> expectedActivities = [ActivityType.Workout];
        var expectedDescription = "A test spot for integration tests";

        var id = SpotId.FromValue(_spotId);
        var searcher = _serviceProvider.GetRequiredService<ISpotSearcher>();

        // Act
        var result = await searcher.GetAsync(id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(id.Value).And.Be(_spotId);
        result.Title.Should().Be(expectedTitle);
        result.Location.Should().Be(expectedLocation);
        result.Activities.Should().BeEquivalentTo(expectedActivities);
        result.Description.Should().Be(expectedDescription);
        result.Address.Should().BeNull();
    }

    [Fact]
    public async Task GetSpot_WithAddress_ShouldReturnData()
    {
        // Arrange
        var expectedTitle = "Test Spot with address";
        var expectedLocation = new GeoLocationDto(52.616670, 39.600000);
        IEnumerable<ActivityType> expectedActivities = [ActivityType.Workout];
        var expectedDescription = "A test spot with address for integration tests";

        var id = SpotId.FromValue(_spotWithAddressId);
        var searcher = _serviceProvider.GetRequiredService<ISpotSearcher>();

        // Act
        var result = await searcher.GetAsync(id, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(id.Value).And.Be(_spotWithAddressId);
        result.Title.Should().Be(expectedTitle);
        result.Location.Should().Be(expectedLocation);
        result.Activities.Should().BeEquivalentTo(expectedActivities);
        result.Description.Should().Be(expectedDescription);
        result.Address.Should().NotBeNull();
        result.Address!.Settlement.Should().Be(LinkedCityName);
    }

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

    [Fact]
    public async Task SearchSpots_WhenAddressMatches_ShouldReturnData()
    {
        // Arrange
        const string searchQuery = "eSt";
        var searcher = _serviceProvider.GetRequiredService<ISpotSearcher>();

        // Act
        var result = await searcher.SearchByAddress(searchQuery, [], CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().NotBeEmpty();
        result.Should().AllSatisfy(x =>
        {
            x.Address.Should().NotBeNullOrWhiteSpace();
            x.Address!.Contains(searchQuery, StringComparison.OrdinalIgnoreCase).Should().BeTrue();
        });
    }

    private static async Task CreateDatabase(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<GeoContext>();
        await context.Database.EnsureDeletedAsync();
        await context.Database.MigrateAsync();
    }

    private async Task SeedDatabase(IServiceProvider serviceProvider)
    {
        await SeedAddresses(serviceProvider);
        await SeedSpots(serviceProvider);
    }

    private async Task SeedSpots(IServiceProvider serviceProvider)
    {
        var context = serviceProvider.GetRequiredService<GeoContext>();
        var address = await context.Addresses.FirstAsync(a => a.Settlement == LinkedCityName, CancellationToken.None);

        var spot = DomainSpot.Create(
               id: SpotId.FromValue(_spotId),
               title: Title.FromValue("Test Spot"),
               locationPoint: GeoCoordinate.FromLocationWithAltitude(GeoLocation.FromLatLon(52.616670, 39.600000), new Altitude(160)),
               activities: [ActivityType.Workout],
               description: "A test spot for integration tests",
               addressId: null);

        var spotWithAddress = DomainSpot.Create(
                id: SpotId.FromValue(_spotWithAddressId),
                title: Title.FromValue("Test Spot with address"),
                locationPoint: GeoCoordinate.FromLocation(GeoLocation.FromLatLon(52.616670, 39.600000)),
                activities: [ActivityType.Workout],
                description: "A test spot with address for integration tests",
                addressId: AddressId.FromValue(address.Id));

        var creator = serviceProvider.GetRequiredService<ISpotCreator>();
        await creator.BulkInsertSpotsAsync([spot, spotWithAddress], CancellationToken.None);
    }

    private async Task SeedAddresses(IServiceProvider serviceProvider)
    {
        var addressLinked = new CreateAddressDto
        {
            Source = AddressSource.None,
            ExternalId = Guid.NewGuid().ToString(),
            CountryCode = CountryCode,
            Location = GeoCoordinate.FromLocation(
                            GeoLocation.FromLatLon(52.616778, 39.600000))
                            .ToPoint(),
            Settlement = LinkedCityName,
            Street = "Test Street Linked",
            PostalCode = "12345",
            Hash = "TestHashLinked",
        };

        var addressStandalone = new CreateAddressDto
        {
            Source = AddressSource.None,
            ExternalId = Guid.NewGuid().ToString(),
            CountryCode = CountryCode,
            Location = GeoCoordinate.FromLocationWithAltitude(
                            GeoLocation.FromLatLon(52.616778, 39.600000),
                            new Altitude(169))
                            .ToPoint(),
            Settlement = "Test City Standalone",
            Street = "Test Street Standalone",
            PostalCode = "54321",
            Hash = "TestHashStandalone",
        };

        var addressCreator = serviceProvider.GetRequiredService<IAddressCreator>();
        await addressCreator.UpsertAddresses([addressLinked, addressStandalone], CancellationToken.None);
    }

    private AsyncServiceScope CreateAsyncScope() => _serviceProvider.CreateAsyncScope();
}