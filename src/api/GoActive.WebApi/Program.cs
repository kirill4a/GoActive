using FluentValidation;
using Asp.Versioning;
using GoActive.WebApi.Infrastructure.Endpoints;
using GoActive.Modules.Geo.Application.Commands;
using GoActive.WebApi.Infrastructure.OpenApi;
using GoActive.WebApi.Infrastructure;

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
    options.DefaultApiVersion = Constants.DefaultApiVersion;
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version"));
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = $"'{Constants.ApiVersionPrefix}'{Constants.ApiVersionFormat}";
    options.SubstituteApiVersionInUrl = true;
});

// Register Swagger OpenAPI
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.ConfigureOptions<ConfigureSwaggerOptions>();

var app = builder.Build();

var apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(Constants.DefaultApiVersion)
    .ReportApiVersions()
    .Build();

var versionedGroup = app
    .MapGroup($"api/{Constants.ApiVersionPrefix}{{version:apiVersion}}")
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
                $"{Constants.ApiName} {description.GroupName}");
        }
    });
}

app.UseHttpsRedirection();

app.Run();
