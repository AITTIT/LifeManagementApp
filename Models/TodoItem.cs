using System;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LifeManagementApp.Models;

// One task in the list. ObservableObject, because IsDone changes while the row is on screen.
public partial class TodoItem : ObservableObject
{
    public string Title { get; set; } = "";
    public string Notes { get; set; } = "";
    public string Category { get; set; } = "";
    public DateTime DueDate { get; set; }
    public int Priority { get; set; }
    public bool Reminder { get; set; }

    [ObservableProperty]
    public partial bool IsDone { get; set; }

    // second line of a row in the list, e.g. "Study · due 03.10.2026 · P4"
    public string Details => $"{Category} - due {DueDate:dd.MM.yyyy} - P{Priority}";


}