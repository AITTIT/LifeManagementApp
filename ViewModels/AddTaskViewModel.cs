using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LifeManagementApp.Models;
using LifeManagementApp.Services;

namespace LifeManagementApp.ViewModels;
public partial class AddTaskViewModel : ObservableObject
{
    // The one shared category list (Settings reads the same one)
    public ObservableCollection<Category> Categories => categoryStore.Items;

    private readonly TodoStore store;
    private readonly CategoryStore categoryStore;
    private readonly IPreferences preferences;

    public AddTaskViewModel(TodoStore store, CategoryStore categoryStore, IPreferences preferences)
    {
        this.store = store;
        this.categoryStore = categoryStore;
        this.preferences = preferences;
        ApplyDefaults();
    }

    public void ApplyDefaults()
    {
        Title = "";
        Notes = "";
        // The default category is saved by name; it is found from the loaded list (empty before the first load).
        string defaultName = preferences.Get(SettingsKeys.DefaultCategory, SettingsDefaults.DefaultCategory);
        SelectedCategory = Categories.FirstOrDefault(c => c.Name == defaultName) ?? Categories.FirstOrDefault();
        Priority = preferences.Get(SettingsKeys.DefaultPriority, SettingsDefaults.DefaultPriority);
        DueDate = DateTime.Today.AddDays(1);
        Reminder = true;
    }

    public async Task OnAppearingAsync()
    {
        await categoryStore.LoadAsync();
        if (string.IsNullOrWhiteSpace(Title) && string.IsNullOrWhiteSpace(Notes))
        {
            ApplyDefaults();
        }
    }

    // ---------- form fields ----------
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TitleCounter))]
    [NotifyPropertyChangedFor(nameof(IsTitleTooLong))]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    public partial string Title { get; set; } = "";

    [ObservableProperty]
    public partial string Notes { get; set; }= "";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    public partial Category? SelectedCategory { get; set; }

    [ObservableProperty]
    public partial DateTime DueDate { get; set; }

    [ObservableProperty]
    public partial double Priority { get; set; }

    [ObservableProperty]
    public partial bool Reminder { get; set; }

    [ObservableProperty]
    public partial string Result { get; set; } = "";

    public string TitleCounter => $"{Title?.Length ?? 0}/40";
    public bool IsTitleTooLong => Title.Length > 40;

    private bool CanSave()
    {
    var title = Title?.Trim();
    return SelectedCategory != null && !string.IsNullOrEmpty(title) && title.Length <= 40;
    }
    
    [RelayCommand(CanExecute = nameof(CanSave))]
    private async Task SaveAsync()  // the generated command is STILL called SaveCommand
    {
        var item = new TodoItem
        {
            Title = Title.Trim(),
            Notes = Notes.Trim(),
            CategoryId = SelectedCategory!.Id,   // what is saved in the database
            Category = SelectedCategory,         // so the list can show the name at once
            DueDate = DueDate,
            Priority = (int)Priority,
            Reminder = Reminder
        };

        try
        {
            await store.AddAsync(item);
        }
        catch (Exception ex)
        {
            Result = $"Could not save: {ex.Message}";
            return;
        }

        Result =    $"Saved: {Title?.Trim()} - {item.CategoryName}" +
                    $" - P{Priority} · due {DueDate:dd.MM.yyyy}" +
                    $" · reminder {(Reminder ? "on" : "off")} ({store.Items.Count} in list)";


        ApplyDefaults();
    }

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddCategoryCommand))]
    public partial string NewCategory { get; set; } = "";

    private bool CanAddCategory()
    {
        var newCategory = NewCategory?.Trim();
        return !string.IsNullOrEmpty(newCategory) &&
                !Categories.Any(cat => string.Equals(newCategory, cat.Name, StringComparison.OrdinalIgnoreCase));
    }

    [RelayCommand(CanExecute = nameof(CanAddCategory))]
    private async Task AddCategoryAsync()  // the generated command is AddCategoryCommand
    {
        var category = new Category { Name = NewCategory.Trim() };
        try
        {
            await categoryStore.AddAsync(category);
        }
        catch (Exception ex)
        {
            Result = $"Could not add the category: {ex.Message}";
            return;
        }

        SelectedCategory = category;
        NewCategory = "";
    }

    [RelayCommand]
    private void Clear()
    {
        ApplyDefaults();
    }

}