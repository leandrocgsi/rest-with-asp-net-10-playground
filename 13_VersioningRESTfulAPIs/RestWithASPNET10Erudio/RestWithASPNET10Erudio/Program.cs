using RestWithASPNET10Erudio.Configurations;
using RestWithASPNET10Erudio.Repositories;
using RestWithASPNET10Erudio.Repositories.Impl;
using RestWithASPNET10Erudio.Services;
using RestWithASPNET10Erudio.Services.Impl;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogLogging();

builder.Services.AddApiVersioningConfiguration();

builder.Services.AddControllers();

// Minimal OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApiSpecification();
builder.Services.AddSwaggerSpecification();

builder.Services.AddDatabaseConfiguration(builder.Configuration);
builder.Services.AddEvolveConfiguration(builder.Configuration, builder.Environment);

builder.Services.AddScoped<IBookServices, BookServicesImpl>();
builder.Services.AddScoped<IPersonServices, PersonServicesImpl>();
builder.Services.AddScoped<PersonServicesImplV2>();

builder.Services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.UseSwaggerSpecification();
app.UseScalarConfiguration();

app.Run();
