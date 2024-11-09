using FluentValidation;
using Asp.Versioning;

using GoActive.WebApi.Infrastructure.Endpoints;
using GoActive.Modules.Geo.Application.Commands;

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
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = ApiVersionReader.Combine(
        new UrlSegmentApiVersionReader(),
        new HeaderApiVersionReader("X-Api-Version"));
})
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;

});

// Register Swagger OpenAPI
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

var apiVersionSet = app.NewApiVersionSet()
    .HasApiVersion(new ApiVersion(1))
    // .HasApiVersion(new ApiVersion(1, status: "preview"))
    // .HasApiVersion(new ApiVersion(DateOnly.FromDateTime(DateTime.Today), status: "preview"))
    .ReportApiVersions()
    .Build();

var versionedGroup = app
    .MapGroup("api/v{version:apiVersion}")
    .WithApiVersionSet(apiVersionSet);

app.MapEndpoints(versionedGroup);

app.Run();
