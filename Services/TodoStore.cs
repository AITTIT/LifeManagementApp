using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Collections.Generic;
using System.Threading.Tasks;
using LifeManagementApp.Models;

namespace LifeManagementApp.Services;

// The app's ONE shared task list (task 8). New: it loads and saves through a repository.
// The ViewModels know only TodoStore – not WHERE the data is stored.
public class TodoStore
{
    private readonly ITodoRepository repository;
    private bool isLoaded;
    public ObservableCollection<TodoItem> Items { get; } = new();

    public TodoStore(ITodoRepository repository)
    {
        this.repository = repository;
    }

    // The store listens to every task it holds.
    private void Watch(TodoItem item)
    {
        item.PropertyChanged += OnItemChanged;
    }

    // async void: an event handler cannot return a Task -> catch everything inside.
    private async void OnItemChanged(object? sender, PropertyChangedEventArgs e)
    {
        // Does comparison as well as assignment (type check and pattern variable binding)
        if (sender is TodoItem item && e.PropertyName == nameof(TodoItem.IsDone))
        {
            try
            {
                await repository.UpdateAsync(item);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Could not save the change: {ex.Message}");
            }
        }
    }

    public async Task LoadAsync()
    {
        if (isLoaded) { return ; } // the Todo list tab appears many times - load only once
        isLoaded = true;

        var tasks = await repository.GetAllAsync();        
        tasks.ForEach(i =>
        {
            Watch(i);
            Items.Add(i);
        });
    }

    public async Task AddAsync(TodoItem item)
    {
        await LoadAsync();
        await repository.AddAsync(item);
        Watch(item);
        Items.Add(item);
    }

    public async Task RemoveAsync(TodoItem item)
    {
        await repository.DeleteAsync(item);
        item.PropertyChanged -= OnItemChanged;
        Items.Remove(item);
    }
}