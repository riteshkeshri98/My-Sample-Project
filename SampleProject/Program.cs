using Microsoft.EntityFrameworkCore;
using SampleProject.Data;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
                     new MySqlServerVersion(new Version(8, 0, 36)),
                        mySqlOptions => mySqlOptions.EnableRetryOnFailure()
                    )); 
builder.Services.AddControllers();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = $"{builder.Configuration["Redis:Host"]}:{builder.Configuration["Redis:Port"]}";
});
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = $"{builder.Configuration["Redis:Host"]}:{builder.Configuration["Redis:Port"]}";
    return ConnectionMultiplexer.Connect(configuration);
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.WebHost.UseUrls("http://0.0.0.0:80");
var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    try
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate(); // Applies any pending migrations
    }
    catch (Exception ex)
    {
        // Log the error or handle it as needed
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
