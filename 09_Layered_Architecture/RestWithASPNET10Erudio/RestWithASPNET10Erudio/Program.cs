using RestWithASPNET10Erudio.Configurations;
using RestWithASPNET10Erudio.Repositories;
using RestWithASPNET10Erudio.Repositories.Impl;
using RestWithASPNET10Erudio.Services;
using RestWithASPNET10Erudio.Services.Impl;
using RestWithASPNETErudio.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.AddSerilogLogging();

builder.Services.AddControllers();

builder.Services.AddDatabaseConfiguration(builder.Configuration);
builder.Services.AddEvolveMigrations(builder.Configuration, builder.Environment);

builder.Services.AddScoped<IPersonRepository, PersonRepositoryImpl>();
builder.Services.AddScoped<IPersonServices, PersonServicesImpl>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
