using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

// Adiciona o gerador de documentos Swagger/OpenAPI
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Sua API", Version = "v1" });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Habilita o middleware para servir o documento Swagger
    // O documento JSON será servido no caminho padrão: /swagger/v1/swagger.json
    app.UseSwagger();

    // Habilita o middleware para servir a Swagger UI (HTML, CSS, JS)
    app.UseSwaggerUI(c =>
    {
        // A UI está configurada para procurar a documentação JSON no caminho padrão.
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Sua API V1");
        // Define a rota para a página do Swagger UI
        c.RoutePrefix = "swagger-ui";
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
