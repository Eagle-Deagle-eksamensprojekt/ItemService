using ItemServiceAPI.Services;
using Services;
using NLog.Web;
using NLog;
using NLog.Loki;
using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;


var builder = WebApplication.CreateBuilder(args);

var logger = NLog.LogManager.Setup().LoadConfigurationFromAppSettings()
        .GetCurrentClassLogger();
        logger.Debug("init main"); // NLog setup


Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.InvariantCulture; // Set the culture to invariant


// Add services to the container.
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddSingleton<IItemDbRepository, ItemMongoDBService>();

// Registrér at I ønsker at bruge NLOG som logger fremadrettet (før builder.build)
builder.Logging.ClearProviders();
builder.Host.UseNLog();

// Vault-integration
var vaultToken = Environment.GetEnvironmentVariable("VAULT_TOKEN") 
                 ?? throw new Exception("Vault token not found");
var vaultUrl = Environment.GetEnvironmentVariable("VAULT_URL") 
               ?? "http://vault:8200"; // Standard Vault URL

var authMethod = new TokenAuthMethodInfo(vaultToken);
var vaultClientSettings = new VaultClientSettings(vaultUrl, authMethod);
var vaultClient = new VaultClient(vaultClientSettings);

var kv2Secret = await vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(path: "Secrets", mountPoint: "secret");
var jwtSecret = kv2Secret.Data.Data["jwtSecret"]?.ToString() ?? throw new Exception("jwtSecret not found in Vault.");
var jwtIssuer = kv2Secret.Data.Data["jwtIssuer"]?.ToString() ?? throw new Exception("jwtIssuer not found in Vault.");
var mongoConnectionString = kv2Secret.Data.Data["MongoConnectionString"]?.ToString() ?? throw new Exception("MongoConnectionString not found in Vault.");

// Register ItemMongoDBService
builder.Services.AddSingleton<IItemDbRepository>(sp =>
{
    var logger = sp.GetRequiredService<ILogger<ItemMongoDBService>>();
    var configuration = sp.GetRequiredService<IConfiguration>();
    return new ItemMongoDBService(logger, mongoConnectionString, configuration);
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = "http://localhost", // Tilpas efter behov
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret))
        };
    });

builder.Services.AddAuthorization();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication(); // Enable authentication
app.UseAuthorization();

app.MapControllers();

app.Run();
