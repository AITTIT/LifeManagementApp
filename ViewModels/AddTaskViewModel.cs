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
    public ObservableCollection<string> Categories { get; } = new() { "Study", "Home", "Health", "Work" };

    private readonly TodoStore store;
    private readonly IPreferences preferences;

    public AddTaskViewModel(TodoStore store, IPreferences preferences)
    {
        this.store = store;
        this.preferences = preferences;
        ApplyDefaults();
    }

    public void ApplyDefaults()
    {
        Title = "";
        Notes = "";
        Category = preferences.Get(SettingsKeys.DefaultCategory, SettingsDefaults.DefaultCategory);
        Priority = preferences.Get(SettingsKeys.DefaultPriority, SettingsDefaults.DefaultPriority);
        DueDate = DateTime.Today.AddDays(1);
        Reminder = true;
    }

    public void OnAppearing()
    {
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
    public partial string Title { get; set; }

    [ObservableProperty]
    public partial string Notes { get; set; }

    [ObservableProperty]
    public partial string Category { get; set; }

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
    return !string.IsNullOrEmpty(title) && title.Length <= 40;
    }
    
    [RelayCommand(CanExecute = nameof(CanSave))]
    private void Save()
    {
        var item = new TodoItem
        {
            Title = Title.Trim(), Notes = Notes.Trim(),
            Category = Category, DueDate = DueDate,
            Priority = (int)Priority, Reminder = Reminder
        };

        store.Add(item);

        //   Saved: Exam prep · Study · P4 · due 03.10.2026 · reminder on (1 in list)
        Result =    $"Saved: {Title?.Trim()} - {Category}" +
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
                !Categories.Any(cat => string.Equals(newCategory, cat, StringComparison.OrdinalIgnoreCase));
    }

    [RelayCommand(CanExecute = nameof(CanAddCategory))]
    private void AddCategory()
    {
        var newCategory = NewCategory.Trim();
        Categories.Add(newCategory);
        Category = newCategory;
        NewCategory = "";
    }

    [RelayCommand]
    private void Clear()
    {
        ApplyDefaults();
    }

}