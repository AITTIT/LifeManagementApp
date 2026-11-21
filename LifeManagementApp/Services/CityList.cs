using LifeManagementApp.Models;

namespace LifeManagementApp.Services;

// The preset cities in ONE place: the Settings Picker and the weather both use this list.
public static class CityList
{
    public static IReadOnlyList<City> All { get; } = new List<City>
    {
        new("Vaasa", 63.10, 21.62),
        new("Helsinki", 60.17, 24.94),
        new("Tampere", 61.50, 23.76),
        new("Turku", 60.45, 22.27),
        new("Oulu", 65.01, 25.47)
    };

    public static List<string> Names => All.Select(c => c.Name).ToList();

    // An unknown name (e.g. an old preference value) falls back to the default city.
    public static City Find(string name)
    {
        return All.FirstOrDefault(c => c.Name == name)
            ?? All.First(c => c.Name == SettingsDefaults.City);
    }
}
