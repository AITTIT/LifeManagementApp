using LifeManagementApp.ViewModels;

namespace LifeManagementApp;

public partial class MainPage : ContentPage
{
    public MainPage()
    {
        InitializeComponent();
        BindingContext = new AddTaskViewModel();
    }

    // pure UI reaction → an event is still the right tool
    protected override void OnAppearing()
    {
        base.OnAppearing();
        TitleEntry.Focus();
    }
}