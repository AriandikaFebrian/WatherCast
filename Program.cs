using Microsoft.EntityFrameworkCore;
using WeatherAPI.Data;
using Npgsql.EntityFrameworkCore.PostgreSQL;

var builder = WebApplication.CreateBuilder(args);

// Menambahkan konfigurasi HttpClient
builder.Services.AddHttpClient();

// Menambahkan DbContext untuk SQL Server
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
                      ?? Environment.GetEnvironmentVariable("DefaultConnection");

builder.Services.AddDbContext<WeatherDbContext>(options =>
    options.UseNpgsql(connectionString));


// Menambahkan controllers
builder.Services.AddControllers();

// Menentukan port untuk deployment
var port = Environment.GetEnvironmentVariable("PORT") ?? "8000";
builder.WebHost.UseUrls($"http://*:{port}");

// Menambahkan Swagger agar tetap aktif di production
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Mengatur CORS agar frontend bisa mengakses API
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder
            .WithOrigins(
                "https://suitable-leanora-codenec-8941accd.koyeb.app",
                "https://rococo-yeot-bec94a.netlify.app"
            )
            .AllowAnyMethod()
            .AllowAnyHeader());
});


var app = builder.Build();

// **Mengaktifkan Swagger di Production**
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "Weather API V1");
});

// **Mengaktifkan middleware**
// app.UseHttpsRedirection(); // Matikan jika tidak memakai HTTPS di Koyeb
app.UseCors("AllowAll");
app.MapControllers();
app.Run();
