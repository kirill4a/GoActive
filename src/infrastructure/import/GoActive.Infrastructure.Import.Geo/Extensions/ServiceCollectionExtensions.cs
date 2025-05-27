using GoActive.Infrastructure.Import.Geo.Address.OpenAddresses;
using GoActive.Infrastructure.Import.Geo.Models;
using GoActive.Infrastructure.Import.Geo.Spot;

using Microsoft.Extensions.DependencyInjection;

namespace GoActive.Infrastructure.Import.Geo.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOpenAdressesImport(this IServiceCollection services)
    {
        services.AddScoped<IGeoJsonLinesImporter<OpenAddressesImportOptions>, OpenAddressesImporter>();
        return services;
    }

    public static IServiceCollection AddSpotImport(this IServiceCollection services, string importerkey)
    {
        services.AddKeyedScoped<IGeoJsonLinesImporter<BatchImportOptions>, SpotImporter>(importerkey);
        return services;
    }
}
