using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeManagementApp.Models;
using LifeManagementApp.Services;

namespace LifeManagementApp.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly TodoStore store;
    private readonly LatestWeatherService weatherService;
    private readonly IPreferences preferences;

    // Top 3 unfinished tasks, sorted by Priority desc then Due date asc
    public ObservableCollection<TodoItem> Top3 { get; } = new();

    // The latest weather reading (null until the first successful fetch)
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasWeather))]
    public partial WeatherInfo? Weather { get; set; }

    [ObservableProperty]
    public partial string City { get; set; } = "";

    [ObservableProperty]
    public partial bool IsKiteWeather { get; set; }

    [ObservableProperty]
    public partial double WindLimit { get; set; }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(RefreshCommand))]
    public partial bool IsBusy { get; set; }

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasError))]
    public partial string ErrorMessage { get; set; } = "";

    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(HasCacheNote))]
    public partial string CacheNote { get; set; } = "";

    public bool HasWeather => Weather != null;
    public bool HasError => ErrorMessage != "";
    public bool HasCacheNote => CacheNote != "";

    public HomeViewModel(TodoStore store, LatestWeatherService weatherService, IPreferences preferences)
    {
        this.store = store;
        this.weatherService = weatherService;
        this.preferences = preferences;
        store.Items.CollectionChanged += (sender, e) =>
        {
            RefreshTop3();
        };
    }

    public async Task OnAppearingAsync()
    {
        await store.LoadAsync();
        RefreshTop3();
        if (CanRefresh())
        {
            await RefreshAsync();
        }
    }

    // Fetches the weather of the city chosen in Settings. The UI stays responsive while waiting.
    // If the fetch fails, the saved weather is shown with a note; with no saved weather, an error message.
    [RelayCommand(CanExecute = nameof(CanRefresh))]
    private async Task RefreshAsync()
    {
        IsBusy = true;
        ErrorMessage = "";
        CacheNote = "";
        try
        {
            // Read the settings on every refresh so a change in Settings takes effect.
            City = preferences.Get(SettingsKeys.City, SettingsDefaults.City);
            WindLimit = preferences.Get(SettingsKeys.WindLimit, SettingsDefaults.WindLimit);

            LatestWeather latest = await weatherService.GetAsync();
            Weather = latest.Weather;
            IsKiteWeather = latest.Weather.IsKiteWeather(WindLimit);
            if (latest.IsCached)
            {
                CacheNote = $"No connection. Showing saved weather from {latest.FetchedAt:g}.";
            }
        }
        catch (WeatherServiceException ex)
        {
            ErrorMessage = ex.Message;
            Weather = null;
            IsKiteWeather = false;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private bool CanRefresh() => !IsBusy;

    private void RefreshTop3()
    {
        // Get unfinished tasks, sort by priority desc then due date asc, take top 3
        List<TodoItem> top3 = store.Items
            .Where(item => !item.IsDone)
            .ToList();
        
        top3 = TodoOrdering.SortByPriority(top3);
        top3 = top3.Take(3).ToList();

        Top3.Clear();
        foreach (var item in top3)
        {
            Top3.Add(item);
        }
    }
}
