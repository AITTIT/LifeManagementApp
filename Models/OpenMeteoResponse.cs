using System.Text.Json.Serialization;

namespace LifeManagementApp.Models;

// The JSON body of Open-Meteo: { "current": { "time": ..., "temperature_2m": ..., "wind_speed_10m": ... }, ... }
// Only the fields we need are here; the others are ignored.
public class OpenMeteoResponse
{
    [JsonPropertyName("current")]
    public OpenMeteoCurrent? Current { get; set; }
}
