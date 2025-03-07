using System.Text.Json.Serialization;
using FluentValidation;
using Asp.Versioning;
using GoActive.WebApi.Infrastructure.Endpoints;
using GoActive.WebApi.Infrastructure;
using GoActive.WebApi.Infrastructure.Cors;
using GoActive.WebApi.Infrastructure.OpenApi;
using GoActive.Infrastructure.Storage.Geo.DI;
using Microsoft.AspNetCore.Http.Json;

var builder = WebApplication.CreateBuilder(args);

ConfigureOptions(builder.Services);
ConfigureServices(builder.Services, builder.Configuration);

var app = builder.Build();

var apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(Constants.Versions.DefaultApiVersion)
    .ReportApiVersions()
    .Build();

var versionedGroup = app
    .MapGroup($"api/{Constants.Versions.ApiVersionPrefix}{{version:apiVersion}}")
    .WithApiVersionSet(apiVersionSet);

app.MapEndpoints(versionedGroup);

if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        foreach (var description in app.DescribeApiVersions())
        {
            options.SwaggerEndpoint(
                $"/swagger/{description.GroupName}/swagger.json",
                $"{Constants.Versions.ApiName} {description.GroupName}");
        }
    });
}

app.UseHttpsRedirection();
app.UseCors(Constants.Cors.Localhost);

await app.RunAsync();

static void ConfigureOptions(IServiceCollection services)
{
    // Based on https://stackoverflow.com/questions/76643787/how-to-make-enum-serialization-default-to-string-in-minimal-api-endpoints-and-sw/76644093
    services.Configure<JsonOptions>(x => x.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
    services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(x => x.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()));
}

static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
{
    services.AddGeoStorage(configuration);

    // Register services in the container.
    services.AddEndpoints(typeof(Program).Assembly);

    // Register Mediator library
    // Learn more on: https://github.com/martinothamar/Mediator
    services.AddMediator(options => options.ServiceLifetime = ServiceLifetime.Scoped);

    // Register validation
    services.AddValidatorsFromAssemblyContaining<Program>();

    // Register Api Versioning
    services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = Constants.Versions.DefaultApiVersion;
        options.ReportApiVersions = true;
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ApiVersionReader = ApiVersionReader.Combine(
            new UrlSegmentApiVersionReader(),
            new HeaderApiVersionReader("X-Api-Version"));
    })
    .AddApiExplorer(options =>
    {
        options.GroupNameFormat = $"'{Constants.Versions.ApiVersionPrefix}'{Constants.Versions.ApiVersionFormat}";
        options.SubstituteApiVersionInUrl = true;
    });

    // Register CORS
    services.ConfigureCors();

    // Register Swagger OpenAPI
    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    services.AddEndpointsApiExplorer().AddSwaggerGen().ConfigureOptions<ConfigureSwaggerOptions>();
}
