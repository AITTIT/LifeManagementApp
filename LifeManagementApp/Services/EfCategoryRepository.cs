using Microsoft.EntityFrameworkCore;
using LifeManagementApp.Data;
using LifeManagementApp.Models;

namespace LifeManagementApp.Services;

// Stores the categories in SQLite with EF Core.
public class EfCategoryRepository : ICategoryRepository
{
    private readonly IDbContextFactory<AppDbContext> factory;

    public EfCategoryRepository(IDbContextFactory<AppDbContext> factory)
    {
        this.factory = factory;
    }

    public async Task<List<Category>> GetAllAsync()
    {
        await using AppDbContext db = await factory.CreateDbContextAsync();
        return await db.Categories
            .AsNoTracking()
            .OrderBy(c => c.Id)
            .ToListAsync();
    }

    public async Task AddAsync(Category category)
    {
        await using AppDbContext db = await factory.CreateDbContextAsync();
        db.Categories.Add(category);
        await db.SaveChangesAsync();
    }
}
