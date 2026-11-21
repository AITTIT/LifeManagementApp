using LifeManagementApp.Models;
using LifeManagementApp.Services;
using LifeManagementApp.Tests.Fakes;
using LifeManagementApp.ViewModels;

namespace LifeManagementApp.Tests;

public class HomeViewModelTests
{
    private static TodoItem Task(int id, int priority, int dueInDays, bool isDone = false)
    {
        return new TodoItem
        {
            Id = id,
            Title = $"Task {id}",
            Priority = priority,
            DueDate = new DateTime(2026, 1, 1).AddDays(dueInDays),
            IsDone = isDone
        };
    }

    private static HomeViewModel CreateViewModel(
        IEnumerable<TodoItem> tasks, FakeWeatherService weather, FakePreferences preferences)
    {
        TodoStore store = new TodoStore(new InMemoryTodoRepository(tasks));
        return new HomeViewModel(store, new LatestWeatherService(weather, preferences), preferences);
    }

    [Fact]
    public async Task Top3_ShowsOnlyUnfinishedTasks_ByPriorityThenDueDate_MaxThree()
    {
        List<TodoItem> tasks = new()
        {
            Task(1, priority: 2, dueInDays: 1),
            Task(2, priority: 5, dueInDays: 9),
            Task(3, priority: 5, dueInDays: 3),                 // same priority as 2, earlier -> first
            Task(4, priority: 5, dueInDays: 1, isDone: true),   // finished -> not shown
            Task(5, priority: 4, dueInDays: 2),
            Task(6, priority: 1, dueInDays: 1)                  // fourth unfinished priority -> cut off
        };
        HomeViewModel viewModel = CreateViewModel(tasks, new FakeWeatherService(), new FakePreferences());

        await viewModel.OnAppearingAsync();

        Assert.Equal(new[] { 3, 2, 5 }, viewModel.Top3.Select(t => t.Id));
    }

    // The alert needs the wind to be OVER the limit: exactly the limit is not enough.
    // The limit is 5.0 when the user has not changed it, otherwise the value from Preferences.
    [Theory]
    [InlineData(5.0, null, false)]
    [InlineData(5.1, null, true)]
    [InlineData(7.9, 8.0, false)]
    [InlineData(8.0, 8.0, false)]
    [InlineData(8.1, 8.0, true)]
    public async Task KiteAlert_IsShownOnlyWhenWindIsOverTheLimit(double wind, double? limit, bool expectedAlert)
    {
        FakePreferences preferences = new FakePreferences();
        if (limit != null)
        {
            preferences.Set(SettingsKeys.WindLimit, limit.Value);
        }
        FakeWeatherService weather = new FakeWeatherService { WindSpeed = wind };
        HomeViewModel viewModel = CreateViewModel(new List<TodoItem>(), weather, preferences);

        await viewModel.OnAppearingAsync();

        Assert.Equal(expectedAlert, viewModel.IsKiteWeather);
    }
}
