using Microsoft.EntityFrameworkCore;
using TodoApp.API.Data;

var builder = WebApplication.CreateBuilder(args);

// 🔹 Add services to the container
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// 🔹 Register DbContext (SQL Server)
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions =>
        {
            sqlOptions.EnableRetryOnFailure(
                maxRetryCount: 5,            // Retry up to 5 times
                maxRetryDelay: TimeSpan.FromSeconds(10),  // Wait max 10s between retries
                errorNumbersToAdd: null       // Null = default transient errors
            );
        }
    )
);

var app = builder.Build();
// Detect if running in Docker
var isDocker = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
app.Urls.Clear();
app.Urls.Add("http://*:8080");
// 🔹 Configure HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


if (!isDocker)
{
    app.UseHttpsRedirection(); // only outside Docker
     // Only migrate when running locally
    using (var scope = app.Services.CreateScope())
    {
        var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
        db.Database.Migrate();
    }
}

app.UseAuthorization();

// 🔹 Map controller routes
app.MapControllers();

app.Run();