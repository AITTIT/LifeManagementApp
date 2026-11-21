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
    public DbSet<Category> Categories => Set<Category>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Two categories with the same name are not allowed.
        modelBuilder.Entity<Category>()
            .HasIndex(c => c.Name)
            .IsUnique();

        // 1-N: one Category has many Todos; every Todo has exactly one Category.
        // Restrict: a category that still has tasks cannot be deleted.
        modelBuilder.Entity<Category>()
            .HasMany(c => c.Todos)
            .WithOne(t => t.Category)
            .HasForeignKey(t => t.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

        // The categories every new database starts with.
        modelBuilder.Entity<Category>().HasData(
            new Category { Id = 1, Name = "Study" },
            new Category { Id = 2, Name = "Home" },
            new Category { Id = 3, Name = "Health" },
            new Category { Id = 4, Name = "Work" });
    }
}