using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeManagementApp.Services;

namespace LifeManagementApp.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly IPreferences preferences;
    private readonly ThemeService themeService;

    public List<string> Themes { get; } = new() { "System", "Light", "Dark" };

    [ObservableProperty]
    public partial string Theme { get; set; } = "";

    public List<string> Categories { get; } = new()
    {
        "Study",
        "Home",
        "Health",
        "Work"
    };

    [ObservableProperty]
    public partial string DefaultCategory { get; set; } = "";

    [ObservableProperty]
    public partial double DefaultPriority { get; set; } // Stepper.Value is a double

    // Part 3: SortOrder

    public SettingsViewModel(IPreferences preferences, ThemeService themeService)
    {
        this.preferences = preferences;
        this.themeService = themeService;
        Theme = preferences.Get(SettingsKeys.Theme, SettingsDefaults.Theme);
        DefaultCategory = preferences.Get(SettingsKeys.DefaultCategory, SettingsDefaults.DefaultCategory);
        DefaultPriority = preferences.Get(SettingsKeys.DefaultPriority, SettingsDefaults.DefaultPriority);
    }

    // The toolkit calls this after every change of Theme (also from the Picker).
    partial void OnThemeChanged(string value)
    {
        preferences.Set(SettingsKeys.Theme, value);
        themeService.Apply(value);
    }

    partial void OnDefaultCategoryChanged(string value)
    {
        preferences.Set(SettingsKeys.DefaultCategory, value);
    }

    partial void OnDefaultPriorityChanged(double value)
    {
        preferences.Set(SettingsKeys.DefaultPriority, (int)value);
    }
}