using LifeManagementApp.Models;

namespace LifeManagementApp.Services;

public interface IWeatherService
{
    // Returns the current weather at the given place.
    // Throws WeatherServiceException when anything goes wrong (no network, timeout, bad answer).
    Task<WeatherInfo> GetCurrentAsync(double latitude, double longitude);
}
