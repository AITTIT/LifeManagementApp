using LifeManagementApp.Models;
using LifeManagementApp.Services;

namespace LifeManagementApp.Tests.Fakes;

// Returns the wind speed given by the test. No network.
public class FakeWeatherService : IWeatherService
{
    public double WindSpeed { get; set; }

    public Task<WeatherInfo> GetCurrentAsync(double latitude, double longitude)
    {
        return Task.FromResult(new WeatherInfo
        {
            Temperature = 1.0,
            WindSpeed = WindSpeed,
            Time = DateTime.Now
        });
    }
}
