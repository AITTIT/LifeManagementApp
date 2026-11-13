namespace LifeManagementApp;

public partial class MainPage : ContentPage
{
    private readonly Command saveCommand;
    public MainPage()
    {
        InitializeComponent();

        CategoryPicker.ItemsSource = new List<String> { "Study", "Home", "Health", "Work" };
        CategoryPicker.SelectedIndex = 0;
        DueDatePicker.Date = DateTime.Today.AddDays(1);

        saveCommand = new Command(Save, CanSave);
        SaveButton.Command = saveCommand;
        TitleEntry.ReturnCommand = saveCommand;
    }

    private bool CanSave()
    {
        var title = TitleEntry.Text?.Trim();
        return !string.IsNullOrEmpty(title) && title.Length <= 40;
    }

    private void Save()
    {
        var title = TitleEntry.Text?.Trim();

        string category = (string)CategoryPicker.SelectedItem;
        int priority = (int)PriorityStepper.Value;
        DateTime due = DueDatePicker.Date;
        bool reminder = ReminderSwitch.IsToggled;

        string formattedTask =  $"{title} - {category}" +
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

    protected override void OnAppearing()
    {
        base.OnAppearing();
        TitleEntry.Focus();

    }

    private void OnTitleChanged(object? sender, TextChangedEventArgs e)
    {
        var n = e.NewTextValue?.Length ?? 0;
        TitleCounterLabel.Text = $"{n}/40";

        if (n > 40)
            TitleCounterLabel.TextColor = Colors.Red;
        else
            TitleCounterLabel.TextColor = Colors.Gray;

        saveCommand.ChangeCanExecute();
    }

}