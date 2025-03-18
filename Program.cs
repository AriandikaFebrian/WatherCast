using Microsoft.EntityFrameworkCore;
using WeatherAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Menambahkan konfigurasi HttpClient
builder.Services.AddHttpClient();

// Menambahkan DbContext untuk SQL Server
builder.Services.AddDbContext<WeatherDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));  // Ganti "DefaultConnection" dengan string koneksi di appsettings.json

// Menambahkan controllers
builder.Services.AddControllers();

var port = Environment.GetEnvironmentVariable("PORT") ?? "8000";
builder.WebHost.UseUrls($"http://*:{port}");


// Menambahkan Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Mengatur CORS untuk frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll",
        builder => builder.AllowAnyOrigin()
                          .AllowAnyMethod()
                          .AllowAnyHeader());
});

var app = builder.Build();

// Mengonfigurasi middleware untuk Swagger jika di lingkungan pengembangan
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Mengaktifkan HTTPS Redirection
//app.UseHttpsRedirection();

// Menambahkan CORS untuk memungkinkan frontend mengakses API
app.UseCors("AllowAll");

// Menyambungkan controller API
app.MapControllers();

// Menjalankan aplikasi
app.Run();
