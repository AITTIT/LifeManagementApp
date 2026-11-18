using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeManagementApp.Models;
using LifeManagementApp.Services;

namespace LifeManagementApp.ViewModels;

public partial class TodoListViewModel : ObservableObject
{
    private readonly TodoStore store;

    // The page shows the store's own collection – not a copy!
    public ObservableCollection<TodoItem> Items => store.Items;

    [ObservableProperty]
    public partial string Summary { get; set; } = "";

    public TodoListViewModel(TodoStore store)
    {
        this.store = store;
        store.Items.CollectionChanged += (sender, e) => UpdateSummary();
        UpdateSummary();
    }

    private void UpdateSummary()
    {
        int count = Items.Count;
        string task = Items.Count == 1 ? "task" : "tasks";
        Summary = $"{count} {task}";
    }

    [RelayCommand]
    private void Delete(TodoItem item) // -> DeleteCommand; item = the row's CommandParameter
    {
        store.Remove(item);
    }

    [RelayCommand]
    private void SortByPriority()
    {
        var sorted = Items
            .OrderByDescending(item => item.Priority)
            .ThenBy(item => item.DueDate)
            .ToList();

        Reorder(sorted);    
    }

    [RelayCommand]
    private void SortByDueDate()
    {
        var sorted = Items
            .OrderBy(item => item.DueDate)
            .ToList();

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