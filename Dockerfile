# Stage 1: Build
FROM mcr.microsoft.com/dotnet/sdk:6.0 AS build-env
WORKDIR /app

# Copy dan restore dependencies
COPY *.csproj ./
RUN dotnet restore

# Copy semua source code dan build aplikasi
COPY . .  
RUN dotnet publish -c Release -o out  

# Stage 2: Runtime
FROM mcr.microsoft.com/dotnet/aspnet:6.0
WORKDIR /app
COPY --from=build-env /app/out .

CMD ["dotnet", "testing2.dll"]
