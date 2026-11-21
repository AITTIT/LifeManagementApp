using System.Collections.Generic;
using System.Threading.Tasks;
using LifeManagementApp.Models;

namespace LifeManagementApp.Services;

// WHERE the tasks are stored. TodoStore knows only this interface –
// a SQLite database (EfTodoRepository) is the implementation now; another one (e.g. a JSON file) could be added without changing the store.
public interface ITodoRepository
{
    Task<List<TodoItem>> GetAllAsync();
    Task AddAsync(TodoItem item);       // gives item.Id a value
    Task UpdateAsync(TodoItem item);    // finds the stored task by Id
    Task DeleteAsync(TodoItem item);    // finds the stored task by Id
}