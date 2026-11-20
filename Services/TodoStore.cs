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

    public async Task LoadAsync()
    {
        if (isLoaded)
        {
            return;         // the Todo list tab appears many times - load only once
        }
        isLoaded = true;

        var tasks = await repository.GetAllAsync();        
        tasks.ForEach(Items.Add);
        // OR tasks.ForEach(t => Items.Add(t));
    }

    public async Task AddAsync(TodoItem item)
    {
        await repository.AddAsync(item);
        Items.Add(item);
    }

    public async Task RemoveAsync(TodoItem item)
    {
        await repository.DeleteAsync(item);
        Items.Remove(item);
    }
}