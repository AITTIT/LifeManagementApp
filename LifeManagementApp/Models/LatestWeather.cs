namespace LifeManagementApp.Models;

// A weather reading together with where and when it was fetched.
// This is also what is saved in Preferences as the offline cache (as JSON).
// IsCached is true when the reading comes from the cache instead of a fresh request.
public record LatestWeather(string City, WeatherInfo Weather, DateTime FetchedAt, bool IsCached = false);
