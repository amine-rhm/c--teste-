using NuGet.Protocol.Core.Types;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;

var builder = WebApplication.CreateBuilder(args);

// Add services to the controller.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); 
// Chargement des services Swagger
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IRepositoryFactory, UniversiteDomain.DataAdapters.DataAdaptersFactory.IRepositoryFactory>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    // Démarrage du service de documentation de l'API au lancement du projet en dev
    app.UseSwagger();
    // Démarrage de l'interface graphique de Swagger au lancement du projet en dev
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();