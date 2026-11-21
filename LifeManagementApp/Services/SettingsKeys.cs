namespace LifeManagementApp.Services;

// All preference keys in one place - never type a key string anywhere else.
public static class SettingsKeys
{
    public const string Theme = "theme";        // string: "System" | "Light" | "Dark"
    public const string DefaultCategory = "default_category"; // string
    public const string DefaultPriority = "default_priority"; // int 1-5
    public const string SortOrder = "sort_order";       // string: "Priority" | "Due date" (saved by the Todo list buttons)
    public const string City = "city";                  // string: a name from CityList
    public const string WindLimit = "wind_limit";       // double: kitesurf alert limit, m/s
        public const string WeatherCache = "weather_cache"; // string: JSON of the last LatestWeather (not a setting, not reset)
}