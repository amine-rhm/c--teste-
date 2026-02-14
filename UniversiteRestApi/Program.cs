using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.Entities;
using UniversiteDomain.JeuxDeDonnees;
using UniversiteDomain.UseCases.SecurityUseCases;
using UniversiteEFDataProvider.Data;
using UniversiteEFDataProvider.Entities;
using UniversiteEFDataProvider.Repositories;
using UniversiteEFDataProvider.RepositoryFactories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();

////////// Swagger avec prise en charge token jwt
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Projet Universite", Version = "v1" });

    // Add Bearer Token Authentication
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your token."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});
/////////////////// Fin config Swagger

////////////// Config système de log en console
builder.Services.AddLogging(options =>
{
    options.ClearProviders();
    options.AddConsole();
});
////////////////////Fin Log

///////////// Configuration des connexions à MySql
String connectionString = builder.Configuration.GetConnectionString("MySqlConnection") 
    ?? throw new InvalidOperationException("Connection string 'MySqlConnection' not found.");

// Création du contexte de la base de données
builder.Services.AddDbContext<UniversiteDbContext>(options => options.UseMySQL(connectionString));

//////////////// Fin connexion BD

////////// Sécurisation
builder.Services.AddIdentityCore<UniversiteUser>(options =>
    { 
        // A modifier en prod pour renforcer la sécurité!!! 
        options.Password.RequiredLength = 4;
        options.Password.RequireNonAlphanumeric = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireDigit = false;
        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedAccount = false;
        options.SignIn.RequireConfirmedEmail = false;
        options.SignIn.RequireConfirmedPhoneNumber = false;
    })
    .AddRoles<UniversiteRole>()
    .AddEntityFrameworkStores<UniversiteDbContext>()
    .AddApiEndpoints()
    .AddDefaultTokenProviders();

// ENREGISTREMENT DES REPOSITORIES POUR L'INJECTION DE DÉPENDANCES
builder.Services.AddScoped<IUniversiteRoleRepository, UniversiteRoleRepository>();
builder.Services.AddScoped<IUniversiteUserRepository, UniversiteUserRepository>();

// La factory est rajoutée dans les services
builder.Services.AddScoped<IRepositoryFactory, RepositoryFactory>();

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };
    });

// Création de tous les services qui sont stockés dans app
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

// Configuration du serveur Web
app.UseHttpsRedirection();
app.MapControllers();

// Sécurisation
app.UseAuthentication();
app.UseAuthorization();

// Initialisation de la base de données
ILogger logger = app.Services.GetRequiredService<ILogger<BdBuilder>>();
logger.LogInformation("Chargement des données de test");
using(var scope = app.Services.CreateScope())
{
    UniversiteDbContext context = scope.ServiceProvider.GetRequiredService<UniversiteDbContext>();
    IRepositoryFactory repositoryFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();
    UserManager<UniversiteUser> userManager = scope.ServiceProvider.GetRequiredService<UserManager<UniversiteUser>>();
    RoleManager<UniversiteRole> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<UniversiteRole>>();
    
    BdBuilder seedBD = new BasicBdBuilder(repositoryFactory);
    await seedBD.BuildUniversiteBdAsync();
}


async Task CheckSecuAsync(HttpContext httpContext, IRepositoryFactory factory, string role,  string email, IUniversiteUser? user)
{
    role = "";
    email = "";
    user = null;

    // Récupération des informations de connexion dans la requête http entrante
    ClaimsPrincipal claims = httpContext.User;

    // Faisons nos tests pour savoir si la personne est bien connectée
    if (claims.Identity?.IsAuthenticated != true)
        throw new UnauthorizedAccessException();

    // Récupérons le email de la personne connectée
    var emailClaim = claims.FindFirst(ClaimTypes.Email);
    if (emailClaim == null)
        throw new UnauthorizedAccessException();

    email = emailClaim.Value;
    if (string.IsNullOrEmpty(email))
        throw new UnauthorizedAccessException();

    // Vérifions qu'il est bien associé à un utilisateur référencé
    user = await new FindUniversiteUserByEmailUseCase(factory).ExecuteAsync(email);
    if (user == null)
        throw new UnauthorizedAccessException();

    // Récupérons le rôle de l'utilisateur
    var ident = claims.Identities.FirstOrDefault();
    if (ident == null)
        throw new UnauthorizedAccessException();

    var roleClaim = ident.FindFirst(ClaimTypes.Role);
    if (roleClaim == null)
        throw new UnauthorizedAccessException();

    role = roleClaim.Value;
    if (string.IsNullOrEmpty(role))
        throw new UnauthorizedAccessException();

    // Vérifions que le user a bien le role envoyé via http
    bool isInRole = await new IsInRoleUseCase(factory).ExecuteAsync(email, role);
    if (!isInRole)
        throw new UnauthorizedAccessException();

    // Si tout est passé sans renvoyer d'exception, le user est authentifié et connecté
}
// Exécution de l'application
app.Run();
