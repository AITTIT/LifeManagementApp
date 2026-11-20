using Microsoft.EntityFrameworkCore;
using LifeManagementApp.Models;


namespace LifeManagementApp.Data;

// One session with the database. Each DbSet is one table
public class AppDbContext : DbContext
{
    // The options (SQLite + file path) come from MauiProgram through DI.
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<TodoItem> Todos => Set<TodoItem>();
}