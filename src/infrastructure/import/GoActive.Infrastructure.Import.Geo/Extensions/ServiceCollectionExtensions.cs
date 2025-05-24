using GoActive.Infrastructure.Import.Geo.Address.OpenAddresses;

using Microsoft.Extensions.DependencyInjection;

namespace GoActive.Infrastructure.Import.Geo.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddOpenAdressesImport(this IServiceCollection services)
    {
        services.AddScoped<IGeoJsonLinesImporter<OpenAddressesImportOptions>, OpenAddressesImporter>();
        return services;
    }
}
