# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy project file dan restore dependencies
COPY ["testing2.csproj", "./"]
RUN dotnet restore "./testing2.csproj"

# Copy semua source code dan build aplikasi
COPY . .
RUN dotnet publish "./testing2.csproj" -c Release -o /app/publish

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
COPY --from=build /app/publish .

# Expose port yang digunakan
EXPOSE 8080

# Menjalankan aplikasi
ENTRYPOINT ["dotnet", "testing2.dll"]
