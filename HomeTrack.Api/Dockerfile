# ---------- BUILD STAGE ----------
FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build-env

WORKDIR /src

# Copy csproj riêng để tận dụng cache
COPY HomeTrack.Api/HomeTrack.Api.csproj ./HomeTrack.Api/

# Copy toàn bộ source code
COPY . .

# Đặt thư mục làm việc là nơi chứa project
WORKDIR /src/HomeTrack.Api

# Khôi phục package
RUN dotnet restore

# Publish ứng dụng
RUN dotnet publish "HomeTrack.Api.csproj" -c Release -o /app/publish /p:UseAppHost=false

# ---------- RUNTIME STAGE ----------
FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS runtime-env

WORKDIR /app

# Copy kết quả publish từ stage trước
COPY --from=build-env /app/publish ./

COPY HomeTrack.Api/appsettings.json ./appsettings.json

# Mở port
EXPOSE 8001

RUN ls -la /app && cat /app/appsettings.json

# Cấu hình để ứng dụng lắng nghe mọi địa chỉ
ENV ASPNETCORE_URLS=http://+:8001
ENV ASPNETCORE_ENVIRONMENT=Production

ENTRYPOINT ["dotnet", "HomeTrack.Api.dll"]
