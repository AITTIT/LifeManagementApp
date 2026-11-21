using System.Collections.ObjectModel;
using LifeManagementApp.Models;

namespace LifeManagementApp.Services;

// Shared ordering logic for TodoListViewModel and HomeViewModel
public static class TodoOrdering
{
    // Sort by Priority desc then Due date asc (the default "Priority" sort order)
    public static List<TodoItem> SortByPriority(IEnumerable<TodoItem> items)
    {
        return items
            .OrderByDescending(i => i.Priority)
            .ThenBy(i => i.DueDate)
            .ToList();
    }

    // Sort by Due date asc then Priority desc
    public static List<TodoItem> SortByDueDate(IEnumerable<TodoItem> items)
    {
        return items
            .OrderBy(i => i.DueDate)
            .ThenByDescending(i => i.Priority)
            .ToList();
    }

    // Apply the given sort order
    public static List<TodoItem> Sort(IEnumerable<TodoItem> items, string sortOrder)
    {
        if (sortOrder == "Due date")
        {
            return SortByDueDate(items);
        }
        else
        {
            return SortByPriority(items);
        }
    }
}
