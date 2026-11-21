using System.Text.Json;
using LifeManagementApp.Models;

namespace LifeManagementApp.Services;

// Gives the latest weather for the city chosen in Settings.
// Every successful fetch is saved in Preferences; if the next fetch fails, the saved one is used.
public class LatestWeatherService
{
    private readonly IWeatherService weatherService;
    private readonly IPreferences preferences;

    public LatestWeatherService(IWeatherService weatherService, IPreferences preferences)
    {
        this.weatherService = weatherService;
        this.preferences = preferences;
    }

    // Throws WeatherServiceException only when the fetch failed AND there is no cache for this city.
    public async Task<LatestWeather> GetAsync()
    {
        City place = CityList.Find(preferences.Get(SettingsKeys.City, SettingsDefaults.City));

        try
        {
            WeatherInfo weather = await weatherService.GetCurrentAsync(place.Latitude, place.Longitude);
            LatestWeather fresh = new LatestWeather(place.Name, weather, DateTime.Now);
            preferences.Set(SettingsKeys.WeatherCache, JsonSerializer.Serialize(fresh));
            return fresh;
        }
        catch (WeatherServiceException)
        {
            LatestWeather? cached = LoadCache();
            // The cache is only valid for the city it was fetched for.
            if (cached == null || cached.City != place.Name)
            {
                throw;
            }
            return cached with { IsCached = true };
        }
    }

    private LatestWeather? LoadCache()
    {
        string json = preferences.Get(SettingsKeys.WeatherCache, "");
        if (json == "") return null;

        try
        {
            return JsonSerializer.Deserialize<LatestWeather>(json);
        }
        catch (JsonException)
        {
            return null;    // a broken cache is the same as no cache
        }
    }
}
