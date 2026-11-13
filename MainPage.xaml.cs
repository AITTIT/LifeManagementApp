// ===== MainPage.xaml.cs (skeleton given) =====
using System;
using System.Collections.Generic;
using Microsoft.Maui.Controls;
using Microsoft.Maui.Graphics;

namespace LifeManagementApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();   // controls exist only after this line

        // TODO 1: fill CategoryPicker.ItemsSource with a List<string>:
        //         "Study", "Home", "Health", "Work"

        CategoryPicker.ItemsSource = new List<String> { "Study", "Home", "Health", "Work" };

        // TODO 2: select the first category (SelectedIndex)
        CategoryPicker.SelectedIndex = 0;

        // TODO 3: set DueDatePicker.Date to tomorrow (DateTime.Today.AddDays(1))
        DueDatePicker.Date = DateTime.Today.AddDays(1);
    }

    private void OnSaveClicked(object? sender, EventArgs e)
    {
        // TODO 4: read the title from TitleEntry.Text (trim it).
        //         If it is empty: show "Title is required." in ResultLabel
        //         in red (Colors.Red) and return.

        var title = TitleEntry.Text?.Trim();
        if (string.IsNullOrEmpty(title))
        {
            ResultLabel.Text = "Title is required.";
            ResultLabel.TextColor = Colors.Red;
            return;
        }

        // TODO 5: read the other values:
        //         category  = CategoryPicker.SelectedItem as string
        //         priority  = (int)PriorityStepper.Value       (Value is a double)
        //         due       = DueDatePicker.Date
        //         reminder  = ReminderSwitch.IsToggled

        string category = (string)CategoryPicker.SelectedItem;
        int priority = (int)PriorityStepper.Value;
        DateTime due = DueDatePicker.Date;
        bool reminder = ReminderSwitch.IsToggled;

        // TODO 6: show the summary in ResultLabel in green (Colors.Green):
        //   Saved: Exam prep · Study · P4 · due 03.10.2026 · reminder on
        //   (date format dd.MM.yyyy, "reminder on" / "reminder off")

        string temp = $"{title} - {category}" +
                            $" - P{priority} · due {due:dd.MM.yyyy}" +
                            $" · reminder {(reminder ? "on" : "off")}";
        SavedList.Children.Insert(0, new Border
        {
            Padding = 10,
            Content = new Label { Text = temp }
        });
        // show a confirmation in the ResultLabel as well
        ResultLabel.Text = "Saved: " + temp;
        ResultLabel.TextColor = Colors.Green;
    }

    // Bonus
    private void OnPriorityChanged(object? sender, ValueChangedEventArgs e)
    {
        if (PriorityLabel == null)
        {
            return;
        }

        PriorityLabel.Text = e.NewValue.ToString();
    }


}