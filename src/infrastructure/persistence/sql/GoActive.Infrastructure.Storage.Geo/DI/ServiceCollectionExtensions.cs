using GoActive.Infrastructure.Storage.Geo.Repositories;
using GoActive.Modules.Geo.Application.Spot.Search;

using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace GoActive.Infrastructure.Storage.Geo.DI;

public static class ServiceCollectionExtensions
{
    /// <summary>
    /// Configures geo storage dependencies.
    /// </summary>
    public static IServiceCollection AddGeoStorage(this IServiceCollection services, IConfiguration configuration) =>
        services.AddDbContext(configuration)
                .AddRepositories();

    private static IServiceCollection AddDbContext(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Geo");
        services.AddDbContext<GeoContext>(options => options
                                                        .UseNpgsql(connectionString, x => x.UseNetTopologySuite())
                                                        .UseSnakeCaseNamingConvention());

        services.AddScoped<IGeoContext>(sp => sp.GetRequiredService<GeoContext>());

        return services;
    }

    private static IServiceCollection AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<SpotRepository>()
                .AddScoped<ISpotSearcher>(sp => sp.GetRequiredService<SpotRepository>());

        return services;
    }
}
