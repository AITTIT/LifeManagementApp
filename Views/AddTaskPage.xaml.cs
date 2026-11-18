using LifeManagementApp.ViewModels;

namespace LifeManagementApp.Views;

public partial class AddTaskPage : ContentPage
{
    public AddTaskPage(AddTaskViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    // pure UI reaction → an event is still the right tool
    protected override void OnAppearing()
    {
        base.OnAppearing();
        TitleEntry.Focus();
    }
}