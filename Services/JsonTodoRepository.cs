using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using LifeManagementApp.Models;

namespace LifeManagementApp.Services;

// Stores all tasks in ONE JSON file.
// Every operation: read the whole file -> change the list -> write the whole file.
public class JsonTodoRepository : ITodoRepository
{
    private static readonly JsonSerializerOptions Options = new() {WriteIndented = true };
    private readonly string filePath;

    public JsonTodoRepository()
    {
        filePath = Path.Combine(FileSystem.AppDataDirectory, "todos.json");
        Debug.WriteLine($"Todo file: {filePath}");      // Look for this line in the Output window
    }

    private async Task<List<TodoItem>> ReadAllAsync()
    {
        if (!File.Exists(filePath))
            return new List<TodoItem>();
        
        string json = await File.ReadAllTextAsync(filePath);

        try
        {
            return JsonSerializer
                .Deserialize<List<TodoItem>>(json) ?? new();            
        }
        catch (JsonException ex)
        {
            Debug.WriteLine($"Broken file: {ex.Message}");
            File.Copy(filePath, filePath + ".broken", true);
            return new();
        }
    }

    private async Task WriteAllAsync(List<TodoItem> items)
    {
        string json = JsonSerializer.Serialize(items, Options);
        await File.WriteAllTextAsync(filePath, json);
    }

    public Task<List<TodoItem>> GetAllAsync() => ReadAllAsync();

    public async Task AddAsync(TodoItem item)
    {
        List<TodoItem> items = await ReadAllAsync();
        item.Id = items.Count == 0 ? 1 : items.Max(i => i.Id) + 1;  // largest + 1
        items.Add(item);
        await WriteAllAsync(items);
    }

    public async Task UpdateAsync(TodoItem item)
    {
        var items = await ReadAllAsync();
        int index = items.FindIndex(i => i.Id == item.Id);
        if (index >= 0)
        {
            items[index] = item;
            await WriteAllAsync(items);
        }
    }

    public async Task DeleteAsync(TodoItem item)
    {
        var items = await ReadAllAsync();
        int index = items.FindIndex(i => i.Id == item.Id);
        if (index >= 0)
        {
            items.RemoveAll(i => i.Id == item.Id);
            await WriteAllAsync(items);
        }
    }
}