using Microsoft.EntityFrameworkCore;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteEFDataProvider.Data;
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
string connectionString = builder.Configuration.GetConnectionString("MySqlConnection") 
    ?? throw new InvalidOperationException("Connection string 'MySqlConnection' not found.");
// Création du contexte de la base de données en utilisant Pomelo.EntityFrameworkCore.MySql
builder.Services.AddDbContext<UniversiteDbContext>(options =>
    options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 31))) // Remplacez par votre version MySQL
);
// La factory est rajoutée dans les services de l'application, toujours prête à être utilisée par injection de dépendances
builder.Services.AddScoped<IRepositoryFactory, RepositoryFactory>();

// Création de tous les services qui sont stockés dans app
// app contient tous les objets de notre application
var app = builder.Build();

// Configuration du serveur Web
app.UseHttpsRedirection();
app.MapControllers();

// Configuration de Swagger.
// Commentez les deux lignes ci-dessous pour désactiver Swagger (en production par exemple)
app.UseSwagger();
app.UseSwaggerUI();

// Initialisation de la base de données
// À commenter si vous ne voulez pas vider la base à chaque Run!
using (var scope = app.Services.CreateScope())
{
    // On récupère le logger pour afficher des messages. On l'a mis dans les services de l'application
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<UniversiteDbContext>>();
    // On récupère le contexte de la base de données qui est stocké dans les services
    var context = scope.ServiceProvider.GetRequiredService<UniversiteDbContext>();
    logger.LogInformation("Initialisation de la base de données");
    // Suppression de la BD
    logger.LogInformation("Suppression de la BD si elle existe");
    await context.Database.EnsureDeletedAsync();
    // Recréation des tables vides
    logger.LogInformation("Création de la BD et des tables à partir des entities");
    await context.Database.EnsureCreatedAsync();
}

// Exécution de l'application
app.Run();
