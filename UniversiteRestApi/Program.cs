using Microsoft.EntityFrameworkCore;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.JeuxDeDonnees;
using UniversiteEFDataProvider.Data;
using UniversiteEFDataProvider.RepositoryFactories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

// Mis en place d'un annuaire des services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Configuration du logging
builder.Services.AddLogging(options =>
{
    options.ClearProviders();
    options.AddConsole();
});

// Configuration de la connexion à MySql
string connectionString = builder.Configuration.GetConnectionString("MySqlConnection") 
                          ?? throw new InvalidOperationException("Connection string 'MySqlConnection' not found.");

// Création du contexte de la base de données
builder.Services.AddDbContext<UniversiteDbContext>(options => options.UseMySQL(connectionString));

// La factory est rajoutée dans les services
builder.Services.AddScoped<IRepositoryFactory, RepositoryFactory>();

var app = builder.Build();

// Configuration du serveur Web
app.UseHttpsRedirection();
app.MapControllers();

// Configuration de Swagger
app.UseSwagger();
app.UseSwaggerUI();

// Initialisation de la base de données (à commenter en production)
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<UniversiteDbContext>>();
    DbContext context = scope.ServiceProvider.GetRequiredService<UniversiteDbContext>();
    
    logger.LogInformation("Initialisation de la base de données");
    logger.LogInformation("Suppression de la BD si elle existe");
    await context.Database.EnsureDeletedAsync();
    
    logger.LogInformation("Création de la BD et des tables à partir des entities");
    await context.Database.EnsureCreatedAsync();
}

// Initisation de la base de données
ILogger loggr = app.Services.GetRequiredService<ILogger<BdBuilder>>();
loggr.LogInformation("Chargement des données de test");
using(var scope = app.Services.CreateScope())
{
    UniversiteDbContext context = scope.ServiceProvider.GetRequiredService<UniversiteDbContext>();
    IRepositoryFactory repositoryFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();   
    // C'est ici que vous changez le jeu de données pour démarrer sur une base vide par exemple
    BdBuilder seedBD = new BasicBdBuilder(repositoryFactory);
    await seedBD.BuildUniversiteBdAsync();
}
// Exécution de l'application
app.Run();