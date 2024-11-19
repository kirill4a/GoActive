using Asp.Versioning.ApiExplorer;

using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;

using Swashbuckle.AspNetCore.SwaggerGen;

namespace GoActive.WebApi.Infrastructure.OpenApi;

internal class ConfigureSwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _versionProvider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider versionProvider)
    {
        ArgumentNullException.ThrowIfNull(versionProvider);
        _versionProvider = versionProvider;
    }


    public void Configure(string? name, SwaggerGenOptions options) => Configure(options);

    public void Configure(SwaggerGenOptions options)
    {
        foreach (var description in _versionProvider.ApiVersionDescriptions)
        {
            var formattedVersion =
                $"{Constants.ApiVersionPrefix}{description.ApiVersion.ToString(Constants.ApiVersionFormat)}";

            var openApiInfo = new OpenApiInfo
            {
                Title = $"{Constants.ApiName} {formattedVersion}",
                Version = formattedVersion
            };
            options.SwaggerDoc(description.GroupName, openApiInfo);
        }
    }
}