using System.Configuration;

using FluentAssertions;

using GoActive.Infrastructure.Import.Geo;

using GoActive.Infrastructure.Import.Geo.Address.OpenAddresses;
using GoActive.Infrastructure.Import.Geo.Extensions;
using GoActive.Infrastructure.Import.Geo.Models;
using GoActive.Infrastructure.Storage.Geo;
using GoActive.Infrastructure.Storage.Geo.DI;
using GoActive.Infrastructure.Storage.Geo.Repositories;
using GoActive.Tests.Common;
using GoActive.Tests.Common.Configuration;

using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Microsoft.Extensions.Logging;

using Moq;

using Testcontainers.PostgreSql;

namespace GoActive.Infrastructure.Import.IntegrationTests.Geo.Address;

[Trait("Category", "Integration")]
[Trait("Category", "Geo")]
public sealed class OpenAddressesImporterTests : IAsyncLifetime
{
    private const string CountryCode = "SI";

    private const string ConfigurationSectionName = "TestContainers";
    private readonly Mock<ILogger<OpenAddressesImporter>> _loggerMock = new();
    private readonly Mock<ILogger<AddressRepository>> _loggerRepositoryMock = new();
    private readonly PostgreSqlContainer _postGisContainer;
    private IServiceProvider _serviceProvider = default!;

    public OpenAddressesImporterTests()
    {
        var configuration = new ConfigurationBuilder()
                    .AddUserSecrets<OpenAddressesImporterTests>()
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

    public static TheoryData<string, ImportResult> NotAddressData => new()
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
            {"type":"Feature","geometry":{"type":"Point","coordinates":[30.5,50.5]},"properties":{"title":"Spot 1","activities":[""]}}
            """,
            new ImportResult(Total: 1, Successes: 0, Errors: 1)
        },
    };

    public static TheoryData<string, string, ImportResult> IncorrectAddressData => new()
    {
        // Wrong geometry type
        // Empty hash
        // Empty id
        {
            """
            {"type": "Feature", "properties": {"hash": "34ebd9543f0c0ebf", "id": "100400000152122093"}, "geometry": {"type": "Polygon", "coordinates": [[[14.027420311879496,46.44872945986677],[14.027420311879496,46.29956625478138],[14.25866773695401,46.29956625478138],[14.25866773695401,46.44872945986677],[14.027420311879496,46.44872945986677]]]}}
            {"type": "Feature", "properties": {"hash": " ", "id": "100400000152122093"}, "geometry": {"type": "Point", "coordinates": [15.7038223, 46.2244948]}}
            {"type": "Feature", "properties": {"hash": "34ebd9543f0c0ebf", "id": " "}, "geometry": {"type": "Point", "coordinates": [15.7038223, 46.2244948]}}
            """,
            CountryCode,
            new ImportResult(Total: 3, Successes: 0, Errors: 3)
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
                .AddScoped(_ => _loggerRepositoryMock.Object)
                .AddOpenAdressesImport()
                .BuildServiceProvider();

        await using var scope = CreateAsyncScope();
        await CreateDatabase(scope.ServiceProvider);
    }

    public Task DisposeAsync() => _postGisContainer.DisposeAsync().AsTask();

    [Theory]
    [MemberData(nameof(NotAddressData))]
    public async Task ImportAddresses_WhenNotAddressSource_ShouldImportNone(string source, ImportResult expectedResult)
    {
        // Arrange
        var options = new OpenAddressesImportOptions()
        {
            BatchOptions = default,
            CountryCode = CountryCode,
        };

        var importer = _serviceProvider.GetRequiredService<IGeoJsonLinesImporter<OpenAddressesImportOptions>>();
        using var stream = source.AsMemoryStream();

        // Act
        var result = await importer.Import(stream, options, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(expectedResult);
        await AssertDatabase(async x =>
        {
            (await x.Addresses.AnyAsync()).Should().Be(false);
        });
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("    ")]
    [InlineData("QWERTY")]
    public async Task ImportAddresses_WhenIncorrectCountry_ShouldImportNone(string? countryCode)
    {
        // Arrange
        var expectedResult = new ImportResult(Total: 1, Successes: 0, Errors: 1);
        const string source =
        """
        {"type": "Feature", "properties": {"hash": "273829de05101f79", "number": "8", "street": "Preradovi\u0107eva ulica", "unit": "", "city": "Ljubljana", "district": "Ljubljana", "region": "Osrednjeslovenska", "postcode": "1000", "id": "100400000127363558"}, "geometry": {"type": "Point", "coordinates": [14.5147878, 46.0654864]}}
        """;

        var options = new OpenAddressesImportOptions()
        {
            BatchOptions = default,
            CountryCode = countryCode!,
        };

        var importer = _serviceProvider.GetRequiredService<IGeoJsonLinesImporter<OpenAddressesImportOptions>>();
        using var stream = source.AsMemoryStream();

        // Act
        var result = await importer.Import(stream, options, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(expectedResult);
        await AssertDatabase(async x =>
        {
            (await x.Addresses.AnyAsync()).Should().Be(false);
        });
    }

    [Theory]
    [MemberData(nameof(IncorrectAddressData))]
    public async Task ImportAddresses_WhenIncorrectSource_ShouldImportNone(string source, string countryCode, ImportResult expectedResult)
    {
        // Arrange
        var options = new OpenAddressesImportOptions()
        {
            BatchOptions = default,
            CountryCode = countryCode,
        };

        var importer = _serviceProvider.GetRequiredService<IGeoJsonLinesImporter<OpenAddressesImportOptions>>();
        using var stream = source.AsMemoryStream();

        // Act
        var result = await importer.Import(stream, options, CancellationToken.None);

        // Assert
        result.Should().NotBeNull();
        result.Should().Be(expectedResult);
        await AssertDatabase(async x =>
        {
            (await x.Addresses.AnyAsync()).Should().Be(false);
        });
    }

    [Fact]
    public async Task ImportAddresses_WhenCorrectSource_ShouldBatchImportDone()
    {
        // Arrange
        const string source = """
            
            {"type": "Feature", "properties": {"hash": "34ebd9543f0c0ebf", "number": "23", "street": "Steklarska ulica", "unit": "", "city": "Rogatec", "district": "Rogatec", "region": "Savinjska", "postcode": "3252", "id": "100400000152122093"}, "geometry": {"type": "Point", "coordinates": [15.7038223, 46.2244948]}}
            {"type": "Feature", "properties": {"hash": "b830aa23375a4515", "number": "14", "street": "Laporska cesta", "unit": "", "city": "Polj\u010dane", "district": "Polj\u010dane", "region": "Podravska", "postcode": "2319", "id": "100400000148572062"}, "geometry": {"type": "Point", "coordinates": [15.5782902, 46.3161068]}}
            {"type": "Feature", "properties": {"hash": "2f5de5164257eb65", "number": "15", "street": "Divjakova ulica", "unit": "", "city": "Miklav\u017e na Dravskem polju", "district": "Miklav\u017e na Dravskem polju", "region": "Podravska", "postcode": "2204", "id": "100400000159339500"}, "geometry": {"type": "Point", "coordinates": [15.6902462, 46.5089847]}}
            {"type": "Feature", "properties": {"hash": "563619a076d3055c", "number": "19", "street": "Gri\u010d", "unit": "", "city": "Logatec", "district": "Logatec", "region": "Osrednjeslovenska", "postcode": "1370", "id": "100400000133162879"}, "geometry": {"type": "Point", "coordinates": [14.2292801, 45.9176334]}}
            {"type": "Feature", "properties": {"hash": "273829de05101f79", "number": "8", "street": "Preradovi\u0107eva ulica", "unit": "", "city": "Ljubljana", "district": "Ljubljana", "region": "Osrednjeslovenska", "postcode": "1000", "id": "100400000127363558"}, "geometry": {"type": "Point", "coordinates": [14.5147878, 46.0654864]}}

            {"type": "Feature", "properties": {"hash": "8594426489a8e864", "number": "1", "street": "Cesta na Ljube\u010dno", "unit": "", "city": "Trnovlje pri Celju", "district": "Celje", "region": "Savinjska", "postcode": "3000", "id": "100400000113034809"}, "geometry": {"type": "Point", "coordinates": [15.2974587, 46.256811]}}
            {"type": "Feature", "properties": {"hash": "7ad48ebd6acc60d9", "number": "1", "street": "Av\u00e9g", "unit": "", "city": "Dolina pri Lendavi", "district": "Lendava", "region": "Pomurska", "postcode": "9220", "id": "100400000192425100"}, "geometry": {"type": "Point", "coordinates": [16.50509, 46.5360455]}}
            {"type": "Feature", "properties": {"hash": "b430d94490f38f15", "number": "5", "street": "Pod brezami", "unit": "", "city": "Spodnja Idrija", "district": "Idrija", "region": "Gori\u0161ka", "postcode": "5281", "id": "100400000118051832"}, "geometry": {"type": "Point", "coordinates": [14.029941, 46.0320058]}}
            


            {"type": "Feature", "properties": {"hash": "8d0aec538f933efd", "number": "11a", "street": "Ob potoku", "unit": "", "city": "Ljubljana", "district": "Ljubljana", "region": "Osrednjeslovenska", "postcode": "1000", "id": "100400000128271057"}, "geometry": {"type": "Point", "coordinates": [14.5458106, 46.043336]}}
            {"type": "Feature", "properties": {"hash": "88a41a28c0cb8de9", "number": "43a", "street": "Na Grivi", "unit": "", "city": "Dragomer", "district": "Log-Dragomer", "region": "Osrednjeslovenska", "postcode": "1351", "id": "100400000188011591"}, "geometry": {"type": "Point", "coordinates": [14.3808095, 46.0170259]}}
            {"type": "Feature", "properties": {"hash": "f9f13ace5318e8c5", "number": "5b", "street": "Ledarska ulica", "unit": "", "city": "Ljubljana", "district": "Ljubljana", "region": "Osrednjeslovenska", "postcode": "1000", "id": "100400000241525298"}, "geometry": {"type": "Point", "coordinates": [14.4703976, 46.0752227]}}
            {"type": "Feature", "properties": {"hash": "d31c69fd9d24b8b4", "number": "9", "street": "Ragovo", "unit": "", "city": "Novo mesto", "district": "Novo mesto", "region": "Jugovzhodna Slovenija", "postcode": "8000", "id": "100400000138723337"}, "geometry": {"type": "Point", "coordinates": [15.1763958, 45.808046]}}
                        
            """;
        ushort batchSize = 4;
        var options = new OpenAddressesImportOptions()
        {
            BatchOptions = new BatchImportOptions { BatchSize = batchSize },
            CountryCode = CountryCode,
        };
        var expectedResult = new ImportResult(Total: 12, Successes: 12, Errors: 0);

        var importer = _serviceProvider.GetRequiredService<IGeoJsonLinesImporter<OpenAddressesImportOptions>>();
        using var stream = source.AsMemoryStream();

        // Act
        var result = await importer.Import(stream, options, CancellationToken.None);

        // Assert
        // TODO: assert batches (events) counts
        result.Should().NotBeNull();
        result.Should().Be(expectedResult);
        await AssertDatabase(async x =>
        {
            (await x.Addresses.CountAsync()).Should().Be(expectedResult.Successes);
        });
    }

    [Fact]
    public async Task ImportAddresses_WhenUpsert_ShouldHaveConsistentData()
    {
        // Arrange
        const string firstCountryCode = "HR";
        const string secondCountryCode = CountryCode;
        const string source = """

            {"type": "Feature", "properties": {"hash": "8d0aec538f933efd", "number": "11a", "street": "Ob potoku", "unit": "", "city": "Ljubljana", "district": "Ljubljana", "region": "Osrednjeslovenska", "postcode": "1000", "id": "100400000128271057"}, "geometry": {"type": "Point", "coordinates": [14.5458106, 46.043336]}}
            {"type": "Feature", "properties": {"hash": "88a41a28c0cb8de9", "number": "43a", "street": "Na Grivi", "unit": "", "city": "Dragomer", "district": "Log-Dragomer", "region": "Osrednjeslovenska", "postcode": "1351", "id": "100400000188011591"}, "geometry": {"type": "Point", "coordinates": [14.3808095, 46.0170259]}}
            {"type": "Feature", "properties": {"hash": "f9f13ace5318e8c5", "number": "5b", "street": "Ledarska ulica", "unit": "", "city": "Ljubljana", "district": "Ljubljana", "region": "Osrednjeslovenska", "postcode": "1000", "id": "100400000241525298"}, "geometry": {"type": "Point", "coordinates": [14.4703976, 46.0752227]}}
            {"type": "Feature", "properties": {"hash": "d31c69fd9d24b8b4", "number": "9", "street": "Ragovo", "unit": "", "city": "Novo mesto", "district": "Novo mesto", "region": "Jugovzhodna Slovenija", "postcode": "8000", "id": "100400000138723337"}, "geometry": {"type": "Point", "coordinates": [15.1763958, 45.808046]}}
                        
            """;
        var options = new OpenAddressesImportOptions()
        {
            BatchOptions = default,
            CountryCode = firstCountryCode,
        };
        var expectedResult = new ImportResult(Total: 4, Successes: 4, Errors: 0);

        var importer = _serviceProvider.GetRequiredService<IGeoJsonLinesImporter<OpenAddressesImportOptions>>();
        using var insertStream = source.AsMemoryStream();
        using var updateStream = source.AsMemoryStream();

        // Act
        var insertResult = await importer.Import(insertStream, options, CancellationToken.None);
        await Task.Delay(100);
        var timestamp = DateTime.UtcNow;

        // Assert
        insertResult.Should().NotBeNull();
        insertResult.Should().Be(expectedResult);
        await AssertDatabase(async x =>
        {
            (await x.Addresses.CountAsync(x => x.Country == firstCountryCode)).Should().Be(expectedResult.Successes);
            (await x.Addresses.AnyAsync(x => x.Country == secondCountryCode)).Should().BeFalse();
            (await x.Addresses.AllAsync(x => x.CreatedAt < timestamp && x.UpdatedAt < timestamp)).Should().BeTrue();
        });

        // Arrange
        var idsOnInsert = await ExtractIdsFromDatabase(async x => await x.Addresses.Select(a => a.Id).ToListAsync());

        // Act
        var updateResult = await importer.Import(updateStream, options with { CountryCode = secondCountryCode }, CancellationToken.None);

        // Assert
        updateResult.Should().NotBeNull();
        updateResult.Should().Be(expectedResult);

        var idsOnUpdate = await ExtractIdsFromDatabase(async x => await x.Addresses.Select(a => a.Id).ToListAsync());
        idsOnUpdate.Should().BeEquivalentTo(idsOnInsert);

        await AssertDatabase(async x =>
        {
            (await x.Addresses.AnyAsync(x => x.Country == firstCountryCode)).Should().BeFalse();
            (await x.Addresses.CountAsync(x => x.Country == secondCountryCode)).Should().Be(expectedResult.Successes);
            (await x.Addresses.AllAsync(x => x.CreatedAt < timestamp && x.UpdatedAt > timestamp)).Should().BeTrue();
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

    private async Task<IReadOnlyCollection<Guid>> ExtractIdsFromDatabase(Func<IGeoContext, Task<IReadOnlyCollection<Guid>>> action)
    {
        await using var scope = CreateAsyncScope();
        var context = scope.ServiceProvider.GetRequiredService<IGeoContext>();
        return await action(context);
    }

    private async Task AssertInNewScope(Func<IServiceProvider, Task> action)
    {
        await using var scope = CreateAsyncScope();
        await action(scope.ServiceProvider);
    }
}
