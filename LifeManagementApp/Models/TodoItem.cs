using System;
using System.Text.Json.Serialization;
using CommunityToolkit.Mvvm.ComponentModel;

namespace LifeManagementApp.Models;

// One task in the list. ObservableObject, because IsDone changes while the row is on screen.
public partial class TodoItem : ObservableObject
{
    public int Id { get; set; }      // NEW: 0 = not saved yet; the repository gives the value
    public string Title { get; set; } = "";
    public string Notes { get; set; } = "";
    public int CategoryId { get; set; }          // foreign key: the column in the Todos table
    public Category? Category { get; set; }      // navigation: filled by Include() or by the code that creates the task
    public DateTime DueDate { get; set; }
    public int Priority { get; set; }
    public bool Reminder { get; set; }

    [ObservableProperty]
    public partial bool IsDone { get; set; }

    [JsonIgnore]
    public string CategoryName => Category?.Name ?? "";

    [JsonIgnore]
    public string Details => $"{CategoryName} - due {DueDate:dd.MM.yyyy} - P{Priority}";


}