using System.Reflection;

using Microsoft.Extensions.DependencyInjection.Extensions;

namespace GoActive.WebApi.Infrastructure.Endpoints;

internal static class Extensions
{
    internal static IServiceCollection AddEndpoints(this IServiceCollection services, Assembly assembly)
    {
        var serviceDescriptors = assembly
        .DefinedTypes
        .Where(t => t is { IsAbstract: false, IsInterface: false }
                    && t.IsAssignableTo(typeof(IEndpoint)))
        .Select(t => ServiceDescriptor.Transient(typeof(IEndpoint), t))
        .ToArray();

        services.TryAddEnumerable(serviceDescriptors);

        return services;
    }

    internal static IApplicationBuilder MapEndpoints(this WebApplication app, RouteGroupBuilder? routeGroupBuilder = null)
    {
        var endpoints = app.Services.GetRequiredService<IEnumerable<IEndpoint>>();

        IEndpointRouteBuilder builder =
            routeGroupBuilder is null ? app : routeGroupBuilder;

        foreach (var endpoint in endpoints)
            endpoint.MapEndpoint(builder);

        return app;
    }
}
