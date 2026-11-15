// ===== AddTaskForm.cs (NEW – skeleton given) =====
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Microsoft.Maui.Controls;

namespace LifeManagementApp;
public class AddTaskForm : INotifyPropertyChanged
{
    // ---------- plumbing ----------
    public event PropertyChangedEventHandler? PropertyChanged;

    protected void OnPropertyChanged([CallerMemberName] string? name = null)
        => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

    protected bool Set<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        OnPropertyChanged(name);
        return true;
    }

    // Default value constants
    private const string DefaultTitle = "";
    private const string DefaultNotes = "";
    private const string DefaultCategory = "Study";
    private const double DefaultPriority = 3;
    private const bool DefaultReminder = true;

    private static DateTime DefaultDueDate =>
        DateTime.Today.AddDays(1);

    // ---------- data shown on the page ----------
    public List<string> Categories { get; } = new() { "Study", "Home", "Health", "Work" };

    private string title = DefaultTitle;
    public string Title
    {
        get => title;
        set
        {
            if (Set(ref title, value ?? DefaultTitle))
            {
                OnPropertyChanged(nameof(TitleCounter)); // computed property below
                OnPropertyChanged(nameof(IsTitleTooLong));
                SaveCommand.ChangeCanExecute();
            }
        }
    }

    // Whenever the UI or code reads TitleCounter, 
    // it calculates a new string using the current Title.
    public string TitleCounter => $"{Title.Length}/40";
    public bool IsTitleTooLong => Title.Length > 40;

    private string notes = DefaultNotes;
    public string Notes
    {
        get => notes;
        set
        {
            if (Set(ref notes, value ?? DefaultNotes))
            {
                OnPropertyChanged(nameof(Notes));
            }
        }
    }

    private string category = DefaultCategory;
    public string Category
    {
        get => category;
        set
        {
            if (Set(ref category, value ?? DefaultCategory))
            {
                OnPropertyChanged(nameof(Category));
            }
        }
    }
    
    private DateTime dueDate = DefaultDueDate;
    public DateTime DueDate
    {
        get => dueDate;
        set
        {
            if (Set(ref dueDate, value))
            {
                OnPropertyChanged(nameof(DueDate));
            }
        }
    }

    private double priority = DefaultPriority;
    public double Priority
    {
        get => priority;
        set
        {
            if (Set(ref priority, value))
            {
                OnPropertyChanged(nameof(Priority));
            }
        }
    }
    
    private bool reminder = DefaultReminder;
    public bool Reminder
    {
        get => reminder;
        set
        {
            if (Set(ref reminder, value))
            {
                OnPropertyChanged(nameof(Reminder));
            }
        }
    }

    private string result = "";
    public string Result
    {
        get => result;
        set
        {
            if(Set(ref result, value ?? ""))
            {
                OnPropertyChanged(nameof(Result));
            }
        }
    }


    // ---------- the action ----------
    public Command SaveCommand { get; }
    public Command ClearCommand { get; }

    public AddTaskForm()
    {
        SaveCommand = new Command(Save, CanSave);
        ClearCommand = new Command(Clear);
    }

    private bool CanSave()
    {
        var title = Title?.Trim();
        return !string.IsNullOrEmpty(title) && title.Length <= 40;
    }

    private void Save()
    {
        Result =    $"{Title?.Trim()} - {Category}" +
                    $" - P{Priority} · due {DueDate:dd.MM.yyyy}" +
                    $" · reminder {(Reminder ? "on" : "off")}";
    }

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