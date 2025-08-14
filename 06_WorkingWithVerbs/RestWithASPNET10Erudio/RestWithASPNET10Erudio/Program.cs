using RestWithASPNET10Erudio.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

// Register MathService for dependency injection
builder.Services.AddSingleton<MathService>();

// ou AddScoped se preferir criar uma instância por request

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
