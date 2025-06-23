using HomeTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using HomeTrack.Application.AcprojSupport; // Namespace của Validation.cs
using HomeTrack.Application.Services;
using HomeTrack.Application.Interface;
using Hangfire;
using Hangfire.MemoryStorage;

DotNetEnv.Env.Load(); 

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // Chỉ gọi một lần
builder.Services.AddHttpClient<IGoogleAIStudioModerationService, GoogleAIStudioModerationService>();

builder.ValidateService(); // Giả sử đây là nơi AddAuthentication().AddJwtBearer() được cấu hình
builder.Services.ConfigureServices(); 

builder.Services.AddHangfire(config =>
{
    config.UseMemoryStorage();
});
builder.Services.AddHangfireServer();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader();
    });
});

// Cấu hình DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' (or DATABASE_URL if using .env and mapped) is not configured.");
}
builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseNpgsql(connectionString));


var app = builder.Build();

app.MapGet("/", () => "HomeTrack is running");

var appLogger = app.Services.GetRequiredService<ILogger<Program>>(); // Hoặc app.Logger nếu .NET 7+
appLogger.LogInformation("Application configured. Starting HTTP request pipeline...");

app.UseDefaultFiles();
app.UseStaticFiles(); 

app.UseHangfireDashboard();

RecurringJob.AddOrUpdate<ISubscriptionService>(
    "CheckExpiredSubscriptions",
    x => x.HandleExpiredSubscriptionsAsync(),
    Cron.Daily
);

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HomeTrack API v1");
        c.RoutePrefix = "swagger";
    });
}
else if (app.Environment.IsProduction())
{
    app.UseSwagger(); // Production vẫn có thể bật Swagger nếu bạn muốn
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HomeTrack API v1");
        c.RoutePrefix = "swagger";
    });
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseCors("AllowAll");


app.UseRouting();

app.UseAuthentication(); // Middleware xác thực
app.UseAuthorization();  // Middleware ủy quyền

app.MapControllers();

appLogger.LogInformation("Application is starting...");
app.Run();