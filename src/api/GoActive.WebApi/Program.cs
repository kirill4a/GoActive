using FluentValidation;
using Asp.Versioning;
using GoActive.WebApi.Infrastructure.Endpoints;
using GoActive.Modules.Geo.Application.Sketch.Create;
using GoActive.WebApi.Infrastructure;
using GoActive.WebApi.Infrastructure.Cors;
using GoActive.WebApi.Infrastructure.OpenApi;

var builder = WebApplication.CreateBuilder(args);

// Register services in the container.
builder.Services.AddEndpoints(typeof(Program).Assembly);

// Register MediatR
builder.Services.AddMediatR(options =>
{
    options.RegisterServicesFromAssemblyContaining<CreateSketchCommand>();
});

// Register validation
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// Register Api Versioning
builder.Services.AddApiVersioning(options =>
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
builder.Services.ConfigureCors();

// Register Swagger OpenAPI
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

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
