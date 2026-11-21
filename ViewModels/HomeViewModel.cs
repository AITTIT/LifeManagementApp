using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LifeManagementApp.Models;
using LifeManagementApp.Services;

namespace LifeManagementApp.ViewModels;

public partial class HomeViewModel : ObservableObject
{
    private readonly TodoStore store;

    // Top 3 unfinished tasks, sorted by Priority desc then Due date asc
    public ObservableCollection<TodoItem> Top3 { get; } = new();

    public HomeViewModel(TodoStore store)
    {
        this.store = store;
        store.Items.CollectionChanged += (sender, e) =>
        {
            RefreshTop3();
        };
    }

    public async Task OnAppearingAsync()
    {
        await store.LoadAsync();
    }

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
