using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WeatherAPI.Data;
using WeatherAPI.Models;

namespace WeatherAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class WeatherController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly WeatherDbContext _context;

        public WeatherController(HttpClient httpClient, WeatherDbContext context)
        {
            _httpClient = httpClient;
            _context = context;
        }

        // Endpoint untuk mengambil semua data cuaca
        [HttpGet("all")]
        public async Task<IActionResult> GetAllWeatherData()
        {
            var weatherData = await _context.Weathers.ToListAsync();
            return Ok(weatherData);
        }

        // Endpoint untuk mengambil data cuaca berdasarkan ID
[HttpGet("{id}")]
public async Task<IActionResult> GetWeatherById(int id)
{
    var weather = await _context.Weathers.FindAsync(id);
    if (weather == null)
    {
        return NotFound();
    }
    return Ok(weather);
}


        // Endpoint untuk mengambil cuaca berdasarkan kota
[HttpGet("forecast")]
public async Task<IActionResult> GetWeatherForecast(string city = "Jakarta")
{
    string apiKey = "1201e499176a67bbf3e65350e41cf231"; 
    string url = $"https://api.openweathermap.org/data/2.5/forecast?q={city}&appid={apiKey}&units=metric";

    try
    {
        var response = await _httpClient.GetStringAsync(url);
        if (string.IsNullOrWhiteSpace(response)) 
        {
            return StatusCode(500, "API response is empty");
        }

        var weatherData = JObject.Parse(response);

        if (weatherData["cod"]?.ToString() != "200")
        {
            return NotFound("City not found");
        }

        // Periksa apakah ada data forecast
        var forecastList = weatherData["list"]?.ToObject<JArray>();
        if (forecastList == null || !forecastList.Any())
        {
            return NotFound("No forecast data available");
        }

        // Cari data cuaca untuk besok pukul 12:00 siang
        var tomorrowDate = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-dd");
        var forecast = forecastList
            .FirstOrDefault(f => f["dt_txt"]?.ToString().Contains($"{tomorrowDate} 12:00:00"));

        if (forecast == null)
        {
            return NotFound("No forecast available for tomorrow");
        }

        var weatherInfo = new Weather
        {
            City = city,
            WeatherDescription = forecast["weather"]?[0]["description"]?.ToString(),
            Temperature = forecast["main"]?["temp"]?.ToObject<decimal>() ?? 0m,
            Humidity = forecast["main"]?["humidity"]?.ToString(),
            WindSpeed = forecast["wind"]?["speed"]?.ToObject<decimal>() ?? 0m,
            CloudCoverage = forecast["clouds"]?["all"]?.ToObject<int>() ?? 0,
            Country = weatherData["city"]?["country"]?.ToString(),
            Timestamp = DateTime.UtcNow
        };

        // Simpan ke database
        _context.Weathers.Add(weatherInfo);
        await _context.SaveChangesAsync();

        return Ok(weatherInfo);
    }
    catch (HttpRequestException ex)
    {
        return StatusCode(500, $"Error retrieving weather data: {ex.Message}");
    }
}
        }

        // Endpoint untuk membuat data cuaca baru
        [HttpPost]
        public async Task<IActionResult> CreateWeather([FromBody] Weather weather)
        {
            if (weather == null)
            {
                return BadRequest("Invalid data.");
            }

            _context.Weathers.Add(weather);
            await _context.SaveChangesAsync();

            return CreatedAtAction(nameof(GetWeather), new { id = weather.Id }, weather);
        }

        // Endpoint untuk memperbarui data cuaca berdasarkan ID
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdateWeather(int id, [FromBody] Weather weather)
        {
            if (id != weather.Id)
            {
                return BadRequest("ID mismatch");
            }

            var existingWeather = await _context.Weathers.FindAsync(id);
            if (existingWeather == null)
            {
                return NotFound();
            }

            existingWeather.City = weather.City;
            existingWeather.WeatherDescription = weather.WeatherDescription;
            existingWeather.Temperature = weather.Temperature;
            existingWeather.Humidity = weather.Humidity;
            existingWeather.WindSpeed = weather.WindSpeed;
            existingWeather.CloudCoverage = weather.CloudCoverage;
            existingWeather.Country = weather.Country;
            existingWeather.Timestamp = weather.Timestamp;

            await _context.SaveChangesAsync();
            return NoContent();
        }

        // Endpoint untuk menghapus data cuaca berdasarkan ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteWeather(int id)
        {
            var weather = await _context.Weathers.FindAsync(id);
            if (weather == null)
            {
                return NotFound();
            }

            _context.Weathers.Remove(weather);
            await _context.SaveChangesAsync();

            return NoContent();
        }
    }
}
