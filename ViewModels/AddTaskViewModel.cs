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

    public AddTaskViewModel(TodoStore store)
    {
        this.store = store;
    }

    // Default value constants
    private const string DefaultTitle = "";
    private const string DefaultNotes = "";
    private const string DefaultCategory = "Study";
    private const double DefaultPriority = 3;
    private const bool DefaultReminder = true;    
    private static DateTime DefaultDueDate =>
        DateTime.Today.AddDays(1);


    // ---------- form fields ----------
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(TitleCounter))]
    [NotifyPropertyChangedFor(nameof(IsTitleTooLong))]
    [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
    public partial string Title { get; set; } = DefaultTitle;

    [ObservableProperty]
    public partial string Notes { get; set; } = DefaultNotes;

    [ObservableProperty]
    public partial string Category { get; set; } = DefaultCategory;

    [ObservableProperty]
    public partial DateTime DueDate { get; set; } = DefaultDueDate;

    [ObservableProperty]
    public partial double Priority { get; set; }= DefaultPriority;

    [ObservableProperty]
    public partial bool Reminder { get; set; } = DefaultReminder;

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


        Title = ""; // SaveCommand re-checks CanSave
        Notes = "";
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
        Title = DefaultTitle;
        Notes = DefaultNotes;
        Category = DefaultCategory;
        Priority = DefaultPriority;
        DueDate = DefaultDueDate;
        Reminder = DefaultReminder;
    }

}