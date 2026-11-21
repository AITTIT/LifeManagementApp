using LifeManagementApp.Models;
using LifeManagementApp.Services;

namespace LifeManagementApp.Tests.Fakes;

// Tasks kept in a list, so the tests need no database. The real TodoStore uses this.
public class InMemoryTodoRepository : ITodoRepository
{
    private readonly List<TodoItem> items;

    public InMemoryTodoRepository(IEnumerable<TodoItem> items)
    {
        this.items = items.ToList();
    }

    public Task<List<TodoItem>> GetAllAsync() => Task.FromResult(items.ToList());

    public Task AddAsync(TodoItem item)
    {
        items.Add(item);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(TodoItem item) => Task.CompletedTask;

    public Task DeleteAsync(TodoItem item)
    {
        items.Remove(item);
        return Task.CompletedTask;
    }
}
