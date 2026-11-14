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

    // ---------- data shown on the page ----------
    public List<string> Categories { get; } = new() { "Study", "Home", "Health", "Work" };

    private string title = "";
    public string Title
    {
        get => title;
        set
        {
            if (Set(ref title, value ?? ""))
            {
                OnPropertyChanged(nameof(TitleCounter)); // computed property below
                // TODO 4a: ask SaveCommand to re-check CanSave
                SaveCommand.ChangeCanExecute();
            }
        }
    }

    // Whenever the UI or code reads TitleCounter, 
    // it calculates a new string using the current Title.
    public string TitleCounter => $"{Title.Length}/40";

    // TODO 4b: write these properties in the same style
    //          (private field + public property + Set(ref …) in the setter):
    //   Notes     string    initial value ""
    private string notes = "";
    public string Notes
    {
        get => notes;
        set
        {
            if (Set(ref notes, value ?? ""))
            {
                OnPropertyChanged(nameof(Notes));
            }
        }
    }

    //   Category  string    initial value "Study"
    private string category = "Study";
    public string Category
    {
        get => category;
        set
        {
            if (Set(ref category, value ?? "Study"))
            {
                OnPropertyChanged(nameof(Category));
            }
        }
    }
    
    //   DueDate   DateTime  initial value DateTime.Today.AddDays(1)
    private DateTime dueDate = DateTime.Today.AddDays(1);
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
    //   Priority  double    initial value 3        (Stepper.Value is a double)
    private double priority = 3;
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
    
    //   Reminder  bool      initial value true
    private bool reminder = true;
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

    //   Result    string    initial value ""
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

    public AddTaskForm()
    {
        // TODO 4c: SaveCommand = new Command(Save, CanSave);
        SaveCommand = new Command(Save, CanSave);
    }

    private bool CanSave()
    {
        // TODO 4d: the same rule as in part 3 – but read the Title property,
        // not TitleEntry.Text (this class does not know the page!)
        var title = Title?.Trim();
        return !string.IsNullOrEmpty(title) && title.Length <= 40;
    }

    private void Save()
    {
        // TODO 4e: put the same summary line as before into Result:
        //   Saved: Exam prep · Study · P4 · due 03.10.2026 · reminder on
        Result =    $"{Title?.Trim()} - {Category}" +
                    $" - P{Priority} · due {DueDate:dd.MM.yyyy}" +
                    $" · reminder {(Reminder ? "on" : "off")}";
    }
}