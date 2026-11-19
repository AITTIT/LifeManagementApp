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

    // Part 2: DefaultCategory, DefaultPriority
    // Part 3: SortOrder

    public SettingsViewModel(IPreferences preferences, ThemeService themeService)
    {
        this.preferences = preferences;
        this.themeService = themeService;
        Theme = preferences.Get(SettingsKeys.Theme, SettingsDefaults.Theme);
    }

    // The toolkit calls this after every change of Theme (also from the Picker).
    partial void OnThemeChanged(string value)
    {
        preferences.Set(SettingsKeys.Theme, value);
        themeService.Apply(value);
    }
}