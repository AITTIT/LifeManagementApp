using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace LifeManagementApp.ViewModels
{
    public partial class AddTaskViewModel : ObservableObject
    {
        // ---------- categories (part 3 changes this line!) ----------
        public List<string> Categories { get; } = new() { "Study", "Home", "Health", "Work" };

        // ---------- form fields ----------
        [ObservableProperty]
        [NotifyPropertyChangedFor(nameof(TitleCounter))]
        [NotifyCanExecuteChangedFor(nameof(SaveCommand))]
        public partial string Title { get; set; } = "";

        private bool CanSave()
        {
        var title = Title?.Trim();
        return !string.IsNullOrEmpty(title) && title.Length <= 40;
        }
        
        [RelayCommand(CanExecute = nameof(CanSave))]
        private void Save()
        {
        Result =    $"{Title?.Trim()} - {Category}" +
                    $" - P{Priority} · due {DueDate:dd.MM.yyyy}" +
                    $" · reminder {(Reminder ? "on" : "off")}";
        }

        [ObservableProperty]
        public partial string Notes { get; set; } = "";
        //   category   string    "Study"
        [ObservableProperty]
        public partial string Category { get; set; } = "Study";
        //   dueDate    DateTime  DateTime.Today.AddDays(1)
        [ObservableProperty]
        public partial DateTime DueDate { get; set; } = DateTime.Today.AddDays(1);
        //   priority   double    3        (Stepper.Value is a double)
        [ObservableProperty]
        public partial double Priority { get; set; }= 3;
        //   reminder   bool      true
        [ObservableProperty]
        public partial bool Reminder { get; set; } = true;
        //   result     string    ""
        [ObservableProperty]
        public partial  string Result { get; set; } = "";

        public string TitleCounter => $"{Title?.Length ?? 0}/40";
    }
}