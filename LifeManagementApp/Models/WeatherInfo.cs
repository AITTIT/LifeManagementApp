namespace LifeManagementApp.Models;

// What the app needs from the weather: one reading. Units: degrees Celsius and metres per second.
public class WeatherInfo
{
    public double Temperature { get; set; }
    public double WindSpeed { get; set; }
    public DateTime Time { get; set; }

    // Kitesurf weather = the wind is OVER the limit (exactly the limit is not enough).
    public bool IsKiteWeather(double windLimit) => WindSpeed > windLimit;
}
