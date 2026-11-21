using System.Collections.Generic;
using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeManagementApp.Models;
using LifeManagementApp.Services;

namespace LifeManagementApp.ViewModels;

public partial class SettingsViewModel : ObservableObject
{
    private readonly IPreferences preferences;
    private readonly ThemeService themeService;
    private readonly CategoryStore categoryStore;

    public List<string> Themes { get; } = new() { "System", "Light", "Dark" };

    [ObservableProperty]
    public partial string Theme { get; set; } = "";

    // The names of the shared category list (filled in OnAppearingAsync, when the list is loaded)
    public ObservableCollection<string> Categories { get; } = new();

    [ObservableProperty]
    public partial string DefaultCategory { get; set; } = "";

    [ObservableProperty]
    public partial double DefaultPriority { get; set; } // Stepper.Value is a double

    public List<string> Cities { get; } = CityList.Names;

    [ObservableProperty]
    public partial string City { get; set; } = "";

    [ObservableProperty]
    public partial double WindLimit { get; set; }   // Stepper.Value is a double

    public SettingsViewModel(IPreferences preferences, ThemeService themeService, CategoryStore categoryStore)
    {
        this.preferences = preferences;
        this.themeService = themeService;
        this.categoryStore = categoryStore;
        Theme = preferences.Get(SettingsKeys.Theme, SettingsDefaults.Theme);
        // DefaultCategory is read in OnAppearingAsync: the Picker has no items to select from yet.
        DefaultPriority = preferences.Get(SettingsKeys.DefaultPriority, SettingsDefaults.DefaultPriority);
        City = preferences.Get(SettingsKeys.City, SettingsDefaults.City);
        WindLimit = preferences.Get(SettingsKeys.WindLimit, SettingsDefaults.WindLimit);
    }

    // Categories can be added on the Add task page, so the names are synced every time Settings appears.
    public async Task OnAppearingAsync()
    {
        await categoryStore.LoadAsync();

        bool changed = false;
        foreach (Category category in categoryStore.Items)
        {
            if (!Categories.Contains(category.Name))
            {
                Categories.Add(category.Name);
                changed = true;
            }
        }

        // The Picker forgets its selection when items are added: select the saved one again.
        if (changed || DefaultCategory == "")
        {
            string saved = preferences.Get(SettingsKeys.DefaultCategory, SettingsDefaults.DefaultCategory);
            DefaultCategory = "";
            DefaultCategory = saved;
        }
    }

    [RelayCommand]
    private void Reset()
    {
        Theme = SettingsDefaults.Theme;
        DefaultCategory = SettingsDefaults.DefaultCategory;
        DefaultPriority = SettingsDefaults.DefaultPriority;
        City = SettingsDefaults.City;
        WindLimit = SettingsDefaults.WindLimit;
    }

    // The toolkit calls this after every change of Theme (also from the Picker).
    partial void OnThemeChanged(string value)
    {
        preferences.Set(SettingsKeys.Theme, value);
        themeService.Apply(value);
    }

    partial void OnDefaultCategoryChanged(string value)
    {
        // The Picker sets null while its list changes - ignore it (and the empty reset value).
        if (string.IsNullOrEmpty(value)) return;
        preferences.Set(SettingsKeys.DefaultCategory, value);
    }

    partial void OnDefaultPriorityChanged(double value)
    {
        preferences.Set(SettingsKeys.DefaultPriority, (int)value);
    }

    partial void OnCityChanged(string value)
    {
        // The Picker sets null while its list is reloaded - ignore it.
        if (string.IsNullOrEmpty(value)) return;
        preferences.Set(SettingsKeys.City, value);
    }

    partial void OnWindLimitChanged(double value)
    {
        preferences.Set(SettingsKeys.WindLimit, value);
    }
}