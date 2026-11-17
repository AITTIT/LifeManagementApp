using System.Collections.ObjectModel;
using LifeManagementApp.Models;

namespace LifeManagementApp.Services;

// The app's ONE shared task list. Both pages get the same object (see MauiProgram).
// Later (SQLite + EF Core) only the inside of this class changes – the pages stay the same.
public class TodoStore
{
    public ObservableCollection<TodoItem> Items { get; } = new();

    public void Add(TodoItem item)
    {
        Items.Add(item);
    }

    public void Remove(TodoItem item)
    {
        Items.Remove(item);
    }
}