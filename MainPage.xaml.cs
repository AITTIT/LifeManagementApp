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
        InitializeComponent();

        CategoryPicker.ItemsSource = new List<String> { "Study", "Home", "Health", "Work" };
        CategoryPicker.SelectedIndex = 0;

        DueDatePicker.Date = DateTime.Today.AddDays(1);
    }

    private void OnSaveClicked(object? sender, EventArgs e)
    {
        var title = TitleEntry.Text?.Trim();
        if (string.IsNullOrEmpty(title))
        {
            ResultLabel.Text = "Title is required.";
            ResultLabel.TextColor = Colors.Red;
            return;
        }

        string category = (string)CategoryPicker.SelectedItem;
        int priority = (int)PriorityStepper.Value;
        DateTime due = DueDatePicker.Date;
        bool reminder = ReminderSwitch.IsToggled;

        string formattedTask = $"{title} - {category}" +
                            $" - P{priority} · due {due:dd.MM.yyyy}" +
                            $" · reminder {(reminder ? "on" : "off")}";
        SavedList.Children.Insert(0, new Border
        {
            Padding = 10,
            Content = new Label { Text = formattedTask }
        });

        ResultLabel.Text = "Saved: " + formattedTask;
        ResultLabel.TextColor = Colors.Green;
    }

    private void OnPriorityChanged(object? sender, ValueChangedEventArgs e)
    {
        if (PriorityLabel == null)
        {
            return;
        }

        PriorityLabel.Text = e.NewValue.ToString();
    }


}