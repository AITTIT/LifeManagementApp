using LifeManagementApp.Models;

namespace LifeManagementApp.Services;

// WHERE the categories are stored (same idea as ITodoRepository).
public interface ICategoryRepository
{
    Task<List<Category>> GetAllAsync();
    Task AddAsync(Category category);       // gives category.Id a value
}
