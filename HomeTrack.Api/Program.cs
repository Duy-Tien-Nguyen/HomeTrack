using HomeTrack.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using HomeTrack.Application.AcprojSupport; // Namespace của Validation.cs
using Microsoft.Extensions.Logging; // Cho ILogger

DotNetEnv.Env.Load(); // Nếu bạn dùng .env

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer(); // Chỉ gọi một lần


// Gọi các phương thức mở rộng tùy chỉnh của bạn
builder.ValidateService(); // Giả sử đây là nơi AddAuthentication().AddJwtBearer() được cấu hình
builder.Services.ConfigureServices(); // Đảm bảo bạn biết rõ phương thức này làm gì

// Cấu hình DbContext
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' (or DATABASE_URL if using .env and mapped) is not configured.");
}
builder.Services.AddDbContext<ApplicationDBContext>(options =>
    options.UseNpgsql(connectionString));


var app = builder.Build();


var appLogger = app.Services.GetRequiredService<ILogger<Program>>(); // Hoặc app.Logger nếu .NET 7+
appLogger.LogInformation("Application configured. Starting HTTP request pipeline...");


if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); 
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "HomeTrack API v1");
        c.RoutePrefix = string.Empty;
    });
    appLogger.LogInformation("Swagger UI configured for Development environment.");
}
else
{
    // app.UseExceptionHandler("/Error");
    // app.UseHsts();
}

// app.UseHttpsRedirection(); // Bật nếu bạn dùng HTTPS

app.UseRouting();

app.UseAuthentication(); // Middleware xác thực
app.UseAuthorization();  // Middleware ủy quyền

app.MapControllers();

appLogger.LogInformation("Application is starting...");
app.Run();