using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using LifeManagementApp.Data;
using LifeManagementApp.Models;

namespace LifeManagementApp.Services;

// Stores the tasks in SQLite with EF Core.
// A fresh AppDbContext for every operation: create -> use -> dispose (await using)
public class EfTodoRepository : ITodoRepository
{
    private readonly IDbContextFactory<AppDbContext> factory;
    public EfTodoRepository(IDbContextFactory<AppDbContext> factory)
    {
        this.factory = factory;
    }

    public async Task<List<TodoItem>> GetAllAsync()
    {
        await using AppDbContext db = await factory.CreateDbContextAsync();
        return await db.Todos
            .AsNoTracking()         // read only: the store owns the objects from now on
            .OrderBy(t => t.Id)
            .ToListAsync();
    }

    public async Task AddAsync(TodoItem item)
    {
        await using AppDbContext db = await factory.CreateDbContextAsync();
        Debug.WriteLine($"Id before Savechanges: {item.Id}");
        db.Todos.Add(item);
        await db.SaveChangesAsync();        // nothing is written before this line
        Debug.WriteLine($"Id after SaveChanges: {item.Id}");
    }

    public async Task UpdateAsync(TodoItem item)
    {
        await using AppDbContext db = await factory.CreateDbContextAsync();
        db.Todos.Update(item);
        await db.SaveChangesAsync();

    }

    public async Task DeleteAsync(TodoItem item)
    {
        await using AppDbContext db = await factory.CreateDbContextAsync();
        db.Todos.Remove(item);
        await db.SaveChangesAsync();
    }
}