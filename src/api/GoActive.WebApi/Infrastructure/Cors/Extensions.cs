namespace GoActive.WebApi.Infrastructure.Cors;

internal static class Extensions
{
    internal static IServiceCollection ConfigureCors(this IServiceCollection services)
    {
        services.AddCors(options =>
        {
            options.AddPolicy(Constants.Cors.Localhost, policy =>
            {
                policy
                    .SetIsOriginAllowed(origin => Uri.TryCreate(origin, UriKind.RelativeOrAbsolute, out var uri) &&
                                                  uri!.Host == "localhost")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        return services;
    }
}
