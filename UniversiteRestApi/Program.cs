using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;

using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.JeuxDeDonnees;
using UniversiteDomain.UseCases.NoteUseCases.Export;
using UniversiteDomain.UseCases.NoteUseCases.Import;
using UniversiteDomain.UseCases.NoteUseCases.Validate;
using UniversiteDomain.UseCases.ParcoursUseCases.Create;
using UniversiteDomain.UseCases.UeUseCases.Create;
using UniversiteEFDataProvider.Data;
using UniversiteEFDataProvider.Entities;
using UniversiteEFDataProvider.RepositoryFactories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
// Mis en place d'un annuaire des services
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddLogging(options =>
{
    options.ClearProviders();
    options.AddConsole();
});

// Configuration de la connexion à MySql
String connectionString = builder.Configuration.GetConnectionString("MySqlConnection") ?? throw new InvalidOperationException("Connection string 'MySqlConnection' not found.");
// Création du contexte de la base de données en utilisant la connexion MySql que l'on vient de définir
// Ce contexte est rajouté dans les services de l'application, toujours prêt à être utilisé par injection de dépendances

builder.Services.AddDbContext<UniversiteDbContext>(options =>
        options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 31))) // Remplacez par votre version MySQL
);
// La factory est rajoutée dans les services de l'application, toujours prête à être utilisée par injection de dépendances
builder.Services.AddScoped<IRepositoryFactory, RepositoryFactory>();
//builder.Services.AddScoped<UserManager<UniversiteUser>>();
//builder.Services.AddScoped<RoleManager<UniversiteRole>>();
builder.Services.AddScoped<ValidationUseCase>();
builder.Services.AddScoped<ExportNotesCsvUseCase>();
builder.Services.AddScoped<ImportNotesCsvUseCase>();

builder.Services.AddScoped<CreateParcoursUseCase>();
builder.Services.AddScoped<CreateUeUseCase>();  // Enregistrement de CreateUeUseCase
builder.Services.AddScoped<CreateNoteUseCase>();  // Enregistrement de CreateNoteUseCase

builder.Services.AddAuthorization();
builder.Services.AddAuthentication(IdentityConstants.ApplicationScheme)
    .AddCookie(IdentityConstants.ApplicationScheme)
    .AddBearerToken(IdentityConstants.BearerScheme);

builder.Services.AddIdentityCore<UniversiteUser>()
    .AddRoles<UniversiteRole>()
    .AddEntityFrameworkStores<UniversiteDbContext>() // Ici, on stocke les users dans la même bd que le reste
    .AddApiEndpoints();

/*builder.Services.AddIdentity<UniversiteUser, UniversiteRole>()
    .AddEntityFrameworkStores<UniversiteDbContext>()
    .AddDefaultTokenProviders();*/

// Création de tous les services qui sont stockés dans app
// app contient tous les aobjets de notre application
var app = builder.Build();

// Configuration du serveur Web
app.UseHttpsRedirection();
app.MapControllers();

// Configuration de Swagger.
// Commentez les deux lignes ci-dessous pour désactiver Swagger (en production par exemple)
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthorization();
// Ajoute les points d'entrée dans l'API pour s'authentifier, se connecter et se déconnecter
app.MapIdentityApi<UniversiteUser>();

// Initisation de la base de données
// A commenter si vous ne voulez pas vider la base à chaque Run!
using(var scope = app.Services.CreateScope())
{
    // On récupère le logger pour afficher des messages. On l'a mis dans les services de l'application
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<UniversiteDbContext>>();
    // On récupère le contexte de la base de données qui est stocké sans les services
    DbContext context = scope.ServiceProvider.GetRequiredService<UniversiteDbContext>();
    logger.LogInformation("Initialisation de la base de données");
    // Suppression de la BD
    logger.LogInformation("Suppression de la BD si elle existe");
    await context.Database.EnsureDeletedAsync();
    // Recréation des tables vides
    logger.LogInformation("Création de la BD et des tables à partir des entities");
    await context.Database.EnsureCreatedAsync();
}
// Initisation de la base de données
ILogger Logger = app.Services.GetRequiredService<ILogger<BdBuilder>>();
Logger.LogInformation("Chargement des données de test");
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


