using Microsoft.EntityFrameworkCore;
using TaskManager.API.Data;
using Swashbuckle.AspNetCore.SwaggerGen;

var builder = WebApplication.CreateBuilder(args);

// 1. Add Services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddOpenApi();

// 2. Configure PostgreSQL Connection (Reads from Docker Environment Variable)
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                       ?? Environment.GetEnvironmentVariable("DATABASE_URL");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' or 'DATABASE_URL' not found.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

// 3. Resilient Database Migration Logic (Retry Loop)
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var logger = services.GetRequiredService<ILogger<Program>>();
    var db = services.GetRequiredService<AppDbContext>();

    int retries = 10;
    while (retries > 0)
    {
        try
        {
            logger.LogInformation("Attempting to apply migrations...");
            db.Database.Migrate();
            logger.LogInformation("Database migrations applied successfully.");
            break; 
        }
        catch (Exception ex)
        {
            retries--;
            logger.LogWarning($"Database not ready yet ({ex.Message}). Retrying in 3 seconds... ({retries} attempts left)");
            if (retries == 0)
            {
                logger.LogCritical(ex, "Could not connect to the database after multiple attempts.");
                throw;
            }
            Thread.Sleep(3000); 
        }
    }
}

// 4. Configure HTTP Pipeline
app.MapOpenApi();
app.UseSwagger();
app.UseSwaggerUI();

// app.UseHttpsRedirection(); // Disabled for local Docker testing

app.MapControllers();

app.Run();