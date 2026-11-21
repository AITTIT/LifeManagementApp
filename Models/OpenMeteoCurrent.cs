using System.Text.Json.Serialization;

namespace LifeManagementApp.Models;

// The nested "current" object of the Open-Meteo response.
public class OpenMeteoCurrent
{
    [JsonPropertyName("time")]
    public DateTime Time { get; set; }

    [JsonPropertyName("temperature_2m")]
    public double Temperature { get; set; }

    [JsonPropertyName("wind_speed_10m")]
    public double WindSpeed { get; set; }
}
