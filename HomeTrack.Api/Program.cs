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

// Cấu hình CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy => // Đổi tên policy để phản ánh mục đích cụ thể hơn
    {
        policy
            .WithOrigins("http://localhost:3000") // Chỉ định nguồn gốc cụ thể
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials(); // Thêm để hỗ trợ gửi cookie/header ủy quyền
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

app.UseCors("AllowSpecificOrigin"); // Sử dụng tên policy đã đổi


app.UseRouting();

app.UseAuthentication(); // Middleware xác thực
app.UseAuthorization();  // Middleware ủy quyền

app.MapControllers();

appLogger.LogInformation("Application is starting...");
app.Run();