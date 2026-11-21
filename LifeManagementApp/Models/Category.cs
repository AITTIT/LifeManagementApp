namespace LifeManagementApp.Models;

// A group of tasks (Study, Home, ...). One category has many tasks: a 1-N relation.
public class Category
{
    public int Id { get; set; }
    public string Name { get; set; } = "";

    // Navigation only: not a column. The foreign key is TodoItem.CategoryId.
    public List<TodoItem> Todos { get; set; } = new();
}
