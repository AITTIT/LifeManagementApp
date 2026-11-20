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

    public ObservableCollection<TodoItem> Items => store.Items;

    [ObservableProperty]
    public partial string Summary { get; set; } = "";

    public TodoListViewModel(TodoStore store, IPreferences preferences)
    {
        this.store = store;
        this.preferences = preferences;
        store.Items.CollectionChanged += (sender, e) => UpdateSummary();
        UpdateSummary();
    }

    private void UpdateSummary()
    {
        int count = Items.Count;
        string task = Items.Count == 1 ? "task" : "tasks";
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
        preferences.Set(SettingsKeys.SortOrder, order);
        ApplySort();
    }

    public void ApplySort()
    {
        string order = preferences.Get(SettingsKeys.SortOrder, SettingsDefaults.SortOrder);
        
        List<TodoItem> sorted;
        if (order == "Due date")
        {
            sorted = Items
                .OrderBy(i => i.DueDate)
                .ThenByDescending(i => i.Priority)
                .ToList();
        }
        else
        {
            sorted = Items
                .OrderByDescending(i => i.Priority)
                .ThenBy(i => i.DueDate)
                .ToList();
        }

        Reorder(sorted);
    }

    private void Reorder(List<TodoItem> sortedItems)
    {
        for (int newIndex = 0; newIndex < sortedItems.Count; newIndex++)
        {
            var item = sortedItems[newIndex];
            int oldIndex = Items.IndexOf(item);

            if (oldIndex != newIndex)
            {
                Items.Move(oldIndex, newIndex);
            }
        }
    }
}