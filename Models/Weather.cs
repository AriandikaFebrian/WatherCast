namespace WeatherAPI.Models
{
    public class Weather
{
    public int Id { get; set; }
    public string City { get; set; }
    public string WeatherDescription { get; set; }
    public decimal Temperature { get; set; }
    public string Humidity { get; set; }
    public decimal WindSpeed { get; set; }
    public int CloudCoverage { get; set; }
    public string Country { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow; // Ubah ke UTC
}

}
