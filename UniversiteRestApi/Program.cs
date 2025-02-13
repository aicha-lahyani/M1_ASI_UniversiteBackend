
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using UniversiteDomain.DataAdapters;
using UniversiteDomain.JeuxDeDonnees;
using UniversiteDomain.UseCases.EtudiantUseCases.Create;
using UniversiteDomain.UseCases.EtudiantUseCases.Delete;
using UniversiteDomain.UseCases.EtudiantUseCases.Get;
using UniversiteEFDataProvider.Data;
using UniversiteEFDataProvider.Entities;
using UniversiteEFDataProvider.RepositoryFactories;
using UniversiteEFDataProvider.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Mvc;
using Université_Domain.UseCases.EtudiantUseCases.Update;
using UniversiteDomain.DataAdapters.DataAdaptersFactory;
using UniversiteDomain.UseCases.ParcoursUseCases.Delete;

using UniversiteDomain.UseCases.ParcoursUseCases.Update;
using UniversiteDomain.UseCases.UeUseCases.Create;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddControllers();
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

builder.Services.AddDbContext<UniversiteDbContext>(options =>options.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 31))));
builder.Services.AddScoped<IRepositoryFactory, RepositoryFactory>();
builder.Services.AddScoped<IEtudiantRepository, EtudiantRepository>();
builder.Services.AddScoped<IUeRepository, UeRepository>();
builder.Services.AddScoped<IParcoursRepository, ParcoursRepository>();

builder.Services.AddScoped<CreateEtudiantUseCase>();
builder.Services.AddScoped<DeleteEtudiantUseCase>();
builder.Services.AddScoped<UpdateEtudiantUseCase>();
builder.Services.AddScoped<GetEtudiantCompletUseCase>();
builder.Services.AddScoped<CreateParcoursUseCase>(); 
builder.Services.AddScoped<DeleteParcoursUseCase>();
builder.Services.AddScoped<UpdateParcoursUseCase>();
builder.Services.AddScoped<CreateUeUseCase>();
builder.Services.AddIdentity<UniversiteUser, UniversiteRole>()
    .AddEntityFrameworkStores<UniversiteDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("VotreCléSecrèteTrèsLongueEtSûre")),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddAuthorization();

builder.Services.AddScoped<UserManager<UniversiteUser>>();
builder.Services.AddScoped<RoleManager<UniversiteRole>>();
builder.Services.AddSingleton<IEmailSender<UniversiteUser>, IdentityNoOpEmailSender>();

var app = builder.Build();


app.UseRouting();
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();


app.MapPost("/custom-login", async ([FromBody] LoginDto loginDto, UserManager<UniversiteUser> userManager, RoleManager<UniversiteRole> roleManager) =>
{
    var user = await userManager.FindByEmailAsync(loginDto.Email);
    if (user != null && await userManager.CheckPasswordAsync(user, loginDto.Password))
    {
        
        var userRoles = await userManager.GetRolesAsync(user);
        var authClaims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        
        foreach (var role in userRoles)
        {
            authClaims.Add(new Claim(ClaimTypes.Role, role));
        }

        var authSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("VotreCléSecrèteTrèsLongueEtSûre"));

        var token = new JwtSecurityToken(
            expires: DateTime.Now.AddHours(3),
            claims: authClaims,
            signingCredentials: new SigningCredentials(authSigningKey, SecurityAlgorithms.HmacSha256)
        );

        return Results.Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token),
            expiration = token.ValidTo
        });
    }
    return Results.Unauthorized();
});



app.MapIdentityApi<UniversiteUser>();


if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var logger = scope.ServiceProvider.GetRequiredService<ILogger<UniversiteDbContext>>();
        var context = scope.ServiceProvider.GetRequiredService<UniversiteDbContext>();

        logger.LogInformation("Initialisation de la base de données");
        await context.Database.EnsureDeletedAsync();
        await context.Database.EnsureCreatedAsync();
    }
}

using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<UniversiteDbContext>();
    var repositoryFactory = scope.ServiceProvider.GetRequiredService<IRepositoryFactory>();

    var seedBD = new BasicBdBuilder(repositoryFactory);
    await seedBD.BuildUniversiteBdAsync();
}


app.Run();


public class IdentityNoOpEmailSender : IEmailSender<UniversiteUser>
{
    public Task SendConfirmationLinkAsync(UniversiteUser user, string email, string confirmationLink) => Task.CompletedTask;
    public Task SendPasswordResetLinkAsync(UniversiteUser user, string email, string resetLink) => Task.CompletedTask;
    public Task SendPasswordResetCodeAsync(UniversiteUser user, string email, string resetCode) => Task.CompletedTask;
}

// DTO pour la connexion
public class LoginDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}