using Microsoft.EntityFrameworkCore;
using SampleProject.Data;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

//read secret files safely
string ReadSecret(string path) =>
    File.Exists(path) ? File.ReadAllText(path).Trim() : string.Empty;

// Load secrets from Docker secrets (mounted at /run/secrets)
var dbUser = ReadSecret("/run/secrets/mysql_user");
var dbPass = ReadSecret("/run/secrets/mysql_root_password");
var dbName = ReadSecret("/run/secrets/mysql_database");
var dbHost = Environment.GetEnvironmentVariable("DB_HOST") ?? "mysql"; // set in docker-compose

// Build MySQL connection string dynamically
var connectionString = $"server={dbHost};port=3306;database={dbName};user={dbUser};password={dbPass}";

// Register MySQL DbContext
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(connectionString,
                     new MySqlServerVersion(new Version(8, 0, 36)),
                     mySqlOptions => mySqlOptions.EnableRetryOnFailure()
    ));

builder.Services.AddControllers();

// Configure Redis
var redisHost = builder.Configuration["Redis:Host"] ?? "redis";
var redisPort = builder.Configuration["Redis:Port"] ?? "6379";
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = $"{redisHost}:{redisPort}";
});
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = $"{redisHost}:{redisPort}";
    return ConnectionMultiplexer.Connect(configuration);
});

// Swagger + hosting
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.WebHost.UseUrls("http://0.0.0.0:80");

var app = builder.Build();

// Run migrations at startup
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"[ERROR] DB migration failed: {ex.Message}");
    }
}

// Middleware
app.UseSwagger();
app.UseSwaggerUI();
app.UseDefaultFiles();
app.UseStaticFiles();
app.UseAuthorization();
app.MapControllers();

app.Run();
