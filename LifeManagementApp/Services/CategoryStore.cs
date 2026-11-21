using System.Collections.ObjectModel;
using LifeManagementApp.Models;

namespace LifeManagementApp.Services;

// The app's ONE shared category list. Both the Add task page and Settings read it from here.
public class CategoryStore
{
    private readonly ICategoryRepository repository;
    private Task? loading;

    public ObservableCollection<Category> Items { get; } = new();

    public CategoryStore(ICategoryRepository repository)
    {
        this.repository = repository;
    }

    // Loads once. Callers that arrive while the load is running wait for the same load.
    public Task LoadAsync()
    {
        loading ??= LoadFromRepositoryAsync();
        return loading;
    }

    private async Task LoadFromRepositoryAsync()
    {
        foreach (Category category in await repository.GetAllAsync())
        {
            Items.Add(category);
        }
    }

    public async Task AddAsync(Category category)
    {
        await LoadAsync();
        await repository.AddAsync(category);
        Items.Add(category);
    }
}
