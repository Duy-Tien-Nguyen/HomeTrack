using Microsoft.Extensions.Logging;
using DotNetEnv;
using HomeTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

DotNetEnv.Env.Load();
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddControllers();
builder.ValidateService();
builder.Services.ConfigureServices();

var connectionString = Environment.GetEnvironmentVariable("DATABASE_URL");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
}   
builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseNpgsql(connectionString));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HomeTrack API v1");
        c.RoutePrefix = string.Empty;
    });
}

var loggerFactory = LoggerFactory.Create(builder =>
{
    builder.AddConsole(); // Ghi log ra console
    builder.SetMinimumLevel(LogLevel.Information); // Mức log tối thiểu
});

ILogger logger = loggerFactory.CreateLogger<Program>();
logger.LogInformation("App is starting...");

// app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();

