using System.Diagnostics;
using System.Globalization;
using System.Net.Http.Json;
using System.Text.Json;
using LifeManagementApp.Models;

namespace LifeManagementApp.Services;

// Fetches the current weather from Open-Meteo (no API key needed).
public class OpenMeteoWeatherService : IWeatherService
{
    private readonly HttpClient http;

    // MauiProgram gives an HttpClient with BaseAddress and Timeout already set.
    public OpenMeteoWeatherService(HttpClient http)
    {
        this.http = http;
    }

    public async Task<WeatherInfo> GetCurrentAsync(double latitude, double longitude)
    {
        // InvariantCulture: the decimal separator must be "." also on a Finnish PC.
        // wind_speed_unit=ms: by default the wind comes in km/h.
        string lat = latitude.ToString("F2", CultureInfo.InvariantCulture);
        string lon = longitude.ToString("F2", CultureInfo.InvariantCulture);
        string url = $"v1/forecast?latitude={lat}&longitude={lon}" +
                     "&current=temperature_2m,wind_speed_10m&wind_speed_unit=ms&timezone=auto";

        try
        {
            using HttpResponseMessage response = await http.GetAsync(url);   // does NOT throw on 4xx/5xx
            Debug.WriteLine($"GET {url} -> {(int)response.StatusCode}");

            if (!response.IsSuccessStatusCode)
            {
                throw new WeatherServiceException($"The weather server answered {(int)response.StatusCode}.");
            }

            OpenMeteoResponse? body = await response.Content.ReadFromJsonAsync<OpenMeteoResponse>();
            if (body?.Current == null)
            {
                throw new WeatherServiceException("The response did not contain weather data.");
            }

            return new WeatherInfo
            {
                Temperature = body.Current.Temperature,
                WindSpeed = body.Current.WindSpeed,
                Time = body.Current.Time
            };
        }
        catch (HttpRequestException ex)
        {
            throw new WeatherServiceException("No connection to the weather server. Check the network.", ex);
        }
        catch (TaskCanceledException ex)        // HttpClient.Timeout
        {
            throw new WeatherServiceException("The weather server did not answer in time.", ex);
        }
        catch (JsonException ex)
        {
            throw new WeatherServiceException("The weather server sent something that is not weather data.", ex);
        }
    }
}
