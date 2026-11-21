using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeManagementApp.Models;
using LifeManagementApp.Services;

namespace LifeManagementApp.ViewModels;

public partial class TodoListViewModel : ObservableObject
{
    private readonly TodoStore store;
    private readonly IPreferences preferences;

    // What the list shows: a sorted copy of the store's items. The store itself keeps its own order.
    public ObservableCollection<TodoItem> Items { get; } = new();

    [ObservableProperty]
    public partial string Summary { get; set; } = "";

    // The chosen sort order. The button of the chosen order is disabled (pressed down).
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanSortByPriority))]
    [NotifyPropertyChangedFor(nameof(CanSortByDueDate))]
    public partial string SortOrder { get; set; } = "";

    public bool CanSortByPriority => SortOrder != "Priority";
    public bool CanSortByDueDate => SortOrder != "Due date";

    public TodoListViewModel(TodoStore store, IPreferences preferences)
    {
        this.store = store;
        this.preferences = preferences;
        SortOrder = preferences.Get(SettingsKeys.SortOrder, SettingsDefaults.SortOrder);
        store.Items.CollectionChanged += (sender, e) =>
        {
            UpdateSummary();
            ApplySort();
        };
        UpdateSummary();
    }

    private void UpdateSummary()
    {
        int count = store.Items.Count;
        string task = count == 1 ? "task" : "tasks";
        Summary = $"{count} {task}";
    }

    // NEW: the page call this in OnAppearing
    public async Task LoadAsync()
    {
        try
        {
            await store.LoadAsync();
        }
        catch (Exception ex)
        {
            Summary = $"Could not load: {ex.Message}";
        }
    }

    // Delete becomes DeleteAsync - the generated command is STILL called DeleteCommand
    [RelayCommand]
    private async Task DeleteAsync(TodoItem item)
    {
        try
        {
            await store.RemoveAsync(item);
        }
        catch (Exception ex)
        {
            Summary = $"Could not delete {ex.Message}";
        }
    }

    [RelayCommand]
    private void SetSortOrder(string order)
    {
        SortOrder = order;
        preferences.Set(SettingsKeys.SortOrder, order);
        ApplySort();
    }

    public void ApplySort()
    {
        List<TodoItem> sorted = TodoOrdering.Sort(store.Items, SortOrder);

        // Rebuild the collection: the list always shows exactly the sorted result.
        Items.Clear();
        foreach (TodoItem item in sorted)
        {
            Items.Add(item);
        }
    }
}